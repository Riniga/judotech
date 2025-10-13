function signOut() 
{
    localStorage.removeItem('currentUser');
    window.location.href = "/";
}

signOut();