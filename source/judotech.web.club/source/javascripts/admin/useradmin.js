/* useradmin.js
 * Kräver:
 *  - DataTables JS (CDN) laddad före detta script
 *  - HTML-struktur från föregående steg (tabell + dialoger)
 */

(() => {
  // ====== Konfiguration: justera vid behov ======
  const API = {
    list:   "http://localhost:7071/api/ReadAllUser",
    create: "http://localhost:7071/api/CreateUser",   // POST
    update: "http://localhost:7071/api/UpdateUser",   // PUT eller POST (upsert)
    delete: "http://localhost:7071/api/DeleteUser"    // DELETE
    // Om din Delete tar t.ex. /DeleteUser?id=... så justera i deleteUser() nedan
  };

  // ====== Hjälpare ======
  const $ = (sel, root = document) => root.querySelector(sel);
  const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));

  const dialogs = {
    add:  $("#dlgAddUser"),
    edit: $("#dlgEditUser"),
    del:  $("#dlgDeleteUser"),
  };

  const forms = {
    add:  $("#formAddUser"),
    edit: $("#formEditUser"),
  };

  const btns = {
    openAdd: $("#btnOpenAddUser"),
    confirmDelete: $("#btnConfirmDelete"),
  };

  const deleteEmailLabel = $("#deleteUserEmail");

  let table;             // DataTable instans
  let users = [];        // Lokal cache av användare
  let selectedUser = null; // För edit/delete

  function openDialog(dlg) {
    if (!dlg.open) dlg.showModal();
  }
  function closeDialog(dlg) {
    if (dlg?.open) dlg.close();
  }
  // PicoCSS-stil: knappar med data-close="dialogId"
  document.addEventListener("click", (e) => {
    const el = e.target.closest("[data-close]");
    if (el) {
      const id = el.getAttribute("data-close");
      const dlg = document.getElementById(id);
      closeDialog(dlg);
    }
  });

  function toDisplay(value) {
    if (value === null || value === undefined || value === "") return "—";
    if (Array.isArray(value)) return value.join(", ");
    return String(value);
  }

  function serializeForm(form) {
    const fd = new FormData(form);
    const obj = Object.fromEntries(fd.entries());

    // Normalisera tomma strängar till null
    for (const k of Object.keys(obj)) {
      if (obj[k] === "") obj[k] = null;
    }

    // Roller: kommaseparerad -> array (eller null)
    if (typeof obj.roles === "string" && obj.roles !== "") {
      obj.roles = obj.roles.split(",").map(s => s.trim()).filter(Boolean);
      if (obj.roles.length === 0) obj.roles = null;
    } else if (obj.roles === null) {
      obj.roles = null;
    }

    return obj;
  }

  async function httpJSON(url, options = {}) {
    const res = await fetch(url, {
      headers: { "Content-Type": "application/json" },
      ...options
    });
    if (!res.ok) {
      const text = await res.text().catch(() => "");
      throw new Error(`HTTP ${res.status}: ${text || res.statusText}`);
    }
    // Vissa endpoints (DELETE) kan svara utan body
    const ct = res.headers.get("content-type") || "";
    if (ct.includes("application/json")) return res.json();
    return null;
  }

  // ====== Data ======
  async function loadUsers() {
    const data = await httpJSON(API.list, { method: "GET" });
    // Säkerställ array
    users = Array.isArray(data) ? data : [];
    return users;
  }

  function getUserByEmail(email) {
    return users.find(u => (u.email || "").toLowerCase() === (email || "").toLowerCase());
  }

  // ====== UI: DataTable ======
  function createTable() {
    // Om tabellen redan fanns – rensa för en ren init
    if (table) {
      table.clear();
      table.rows().remove();
      table.destroy();
      table = null;
    }

    const rows = users.map(u => {
      return {
        email: toDisplay(u.email),
        fullname: toDisplay(u.fullname),
        primaryphone: toDisplay(u.primaryphone),
        license: toDisplay(u.license),
        club: toDisplay(u.club),
        roles: toDisplay(u.roles),
        actions: u.email
      };
    });

    table = new DataTable("#usersTable", {
      data: rows,
      columns: [
        { data: "email", title: "E-post" },
        { data: "fullname", title: "Namn" },
        { data: "primaryphone", title: "Telefon" },
        { data: "license", title: "Licens" },
        { data: "club", title: "Klubb" },
        { data: "roles", title: "Roller" },
        {
          data: "actions",
          title: "Åtgärder",
          orderable: false,
          searchable: false,
          render: (email) => `
            <div class="table-actions">
              <button class="secondary outline btn-edit" data-email="${email}">Redigera</button>
              <button class="contrast btn-delete" data-email="${email}">Radera</button>
            </div>`
        }
      ],
      responsive: true,
      paging: true,
      pageLength: 25,
      language: {
        emptyTable: "Inga användare hittades.",
        search: "Sök:",
        lengthMenu: "Visa _MENU_ rader",
        info: "Visar _START_–_END_ av _TOTAL_",
        paginate: { previous: "Föregående", next: "Nästa" }
      }
    });

    // Delegation för knappar i tabellen
    $("#usersTable").addEventListener("click", onTableClick);
  }

  function refreshTable() {
    if (!table) return createTable();
    const rows = users.map(u => ({
      email: toDisplay(u.email),
      fullname: toDisplay(u.fullname),
      primaryphone: toDisplay(u.primaryphone),
      license: toDisplay(u.license),
      club: toDisplay(u.club),
      roles: toDisplay(u.roles),
      actions: u.email
    }));
    table.clear();
    table.rows.add(rows);
    table.draw();
  }

  // ====== Handlers ======
  function onTableClick(e) {
    const editBtn = e.target.closest(".btn-edit");
    const delBtn = e.target.closest(".btn-delete");
    if (editBtn) {
      const email = editBtn.getAttribute("data-email");
      handleOpenEdit(email);
    } else if (delBtn) {
      const email = delBtn.getAttribute("data-email");
      handleOpenDelete(email);
    }
  }

  function fillEditForm(user) {
    // Map 1:1 mot dina formfält
    const set = (name, val) => {
      const el = $(`[name="${name}"]`, forms.edit);
      if (el) el.value = val ?? "";
    };

    set("id", user.id ?? user.email ?? "");
    set("originalEmail", user.email ?? "");
    set("email", user.email ?? "");
    set("fullname", user.fullname ?? "");
    set("personnumber", user.personnumber ?? "");
    set("primaryphone", user.primaryphone ?? "");
    set("secondaryphone", user.secondaryphone ?? "");
    set("license", user.license ?? "");
    set("adress", user.adress ?? "");
    set("postalcode", user.postalcode ?? "");
    set("city", user.city ?? "");
    set("club", user.club ?? "");
    set("zone", user.zone ?? "");
    set("roles", Array.isArray(user.roles) ? user.roles.join(", ") : (user.roles ?? ""));
    // password är tomt vid edit (fylls endast om man byter)
    set("password", "");
  }

  function clearAddForm() {
    forms.add.reset();
  }

  function handleOpenEdit(email) {
    const u = getUserByEmail(email);
    if (!u) {
      alert("Kunde inte hitta användaren.");
      return;
    }
    selectedUser = u;
    fillEditForm(u);
    openDialog(dialogs.edit);
  }

  function handleOpenDelete(email) {
    const u = getUserByEmail(email);
    if (!u) {
      alert("Kunde inte hitta användaren.");
      return;
    }
    selectedUser = u;
    deleteEmailLabel.textContent = u.email || "—";
    openDialog(dialogs.del);
  }

  // ====== CRUD calls ======
  async function createUser(payload) {
    // Förväntat payloadfält: enligt din modell
    // Ex: { email, fullname, personnumber, ... , roles: [..], password }
    return await httpJSON(API.create, {
      method: "POST",
      body: JSON.stringify(payload)
    });
  }

  async function updateUser(payload) {
    // Om din backend kräver "id" (ofta email) – se till att skicka med.
    // Justera method till "POST" om du kör upsert i Azure Function.
    return await httpJSON(API.update, {
      method: "PUT",
      body: JSON.stringify(payload)
    });
  }

  async function deleteUser(emailOrId) {
    // Vanliga varianter – välj en och kommentera den andra.
    // 1) Body i DELETE:
    return await httpJSON(API.delete, {
      method: "DELETE",
      body: JSON.stringify({ id: emailOrId, email: emailOrId })
    });

    // 2) Querystring:
    // return await httpJSON(`${API.delete}?id=${encodeURIComponent(emailOrId)}`, { method: "DELETE" });
  }

  // ====== Event bindningar ======
  btns.openAdd?.addEventListener("click", () => {
    clearAddForm();
    openDialog(dialogs.add);
  });

  forms.add?.addEventListener("submit", async (e) => {
    e.preventDefault();
    const data = serializeForm(forms.add);

    // Minimal validering
    if (!data.email) {
      alert("E-post är obligatoriskt.");
      return;
    }

    try {
      await createUser(data);
      closeDialog(dialogs.add);
      await reload();
      alert("Användare skapad.");
    } catch (err) {
      console.error(err);
      alert("Misslyckades att skapa användare: " + err.message);
    }
  });

  forms.edit?.addEventListener("submit", async (e) => {
    e.preventDefault();
    const data = serializeForm(forms.edit);
    // Säkra id/emailfält för backend
    data.id = data.id || data.email || data.originalEmail;
    console.log(data);
    try {
      await updateUser(data);
      closeDialog(dialogs.edit);
      await reload();
      alert("Användare uppdaterad.");
    } catch (err) {
      console.error(err);
      alert("Misslyckades att uppdatera användare: " + err.message);
    }
  });

  btns.confirmDelete?.addEventListener("click", async (e) => {
    e.preventDefault();
    if (!selectedUser?.email) {
      alert("Ingen användare vald.");
      return;
    }
    try {
      await deleteUser(selectedUser.email);
      closeDialog(dialogs.del);
      await reload();
      alert("Användare raderad.");
    } catch (err) {
      console.error(err);
      alert("Misslyckades att radera användare: " + err.message);
    }
  });

  // ====== Ladda + init ======
  async function reload() {
    await loadUsers();
    refreshTable();
  }

  document.addEventListener("DOMContentLoaded", async () => {
    try {
      await loadUsers();
      createTable();
    } catch (err) {
      console.error(err);
      alert("Kunde inte ladda användare: " + err.message);
    }
  });
})();
