// member.js
(function () {
  // Hjälp: hämta query-param
  function getParam(name) {
    const p = new URLSearchParams(window.location.search);
    return p.get(name);
  }

  // Hjälp: säkert sätta text
  function setText(id, value) {
    
    const el = document.getElementById(id);
    console.log(id, value, el);
    if (el) el.textContent = value ?? '';
  }

  // Hjälp: säkert sätta value
  function setValue(id, value) {
    const el = document.getElementById(id);
    if (el) el.value = value ?? '';
  }

  // (valfritt) formatera ålder (tar heltal om det är .0)
  function formatAge(age) {
    if (age == null || isNaN(Number(age))) return '';
    const n = Number(age);
    return String(Math.floor(n)); 
  }
  


  // Rendera titelrad: "Förnamn Efternamn — NN år"
  function renderTitle(m) {
    const fn = m['förnamn'] ?? '';
    const en = m['efternamn'] ?? '';
    const space = (fn && en) ? ' ' : '';
    const ageStr = formatAge(m['ålder']);
    return `${fn}${space}${en} — ${ageStr ? ageStr + ' år' : ''}`.trim();
  }

  async function init() {
    const id = getParam('id');
    if (!id) {
      console.warn('Ingen id-parameter i URL:en.');
      return;
    }
    let data;
    try {
      const res = await fetch('data/medlemmar.json', { cache: 'no-store' });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      data = await res.json();
    } catch (e) {
      console.error('Kunde inte läsa data/member.json:', e);
      return;
    }

    const member = Array.isArray(data) ? data.find(x => x.id === id) : null;
    
    if (!member) {
      console.warn(`Hittade ingen medlem med id='${id}'.`);
      return;
    }
    console.log(member);

    // Fyll fält
    setText('memberTitle', renderTitle(member));
    setText('beltGrade', member['grad'] ?? '');
    setText('attendance', member['närvaro'] ?? '');
    setValue('comment', "Enligt ålder och närvaro borde graderas till: " +  member['borde'] ?? '');

    // Checkboxar (om du senare lägger till i JSON som true/false)
    const mapChecks = {
      likes_technique: 'likes_technique',
      likes_randori: 'likes_randori',
      likes_compete: 'likes_compete',
      likes_coach: 'likes_coach',
      likes_referee: 'likes_referee'
    };
    for (const [name, key] of Object.entries(mapChecks)) {
      const el = document.querySelector(`input[name="${name}"]`);
      if (el && key in member) el.checked = Boolean(member[key]);
    }
  }

  document.addEventListener('DOMContentLoaded', init);
})();
