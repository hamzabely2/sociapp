import Cookies from 'universal-cookie';
const cookies = new Cookies();
const token = cookies.get("token");

export function decodeToken() {
    try {
        if(token === null)
            return null;

        return JSON.parse(atob(token.split('.')[1]));

    } catch (error) {
        console.error('Erreur lors du décodage du token :', error);
        return null;
    }
}

export const setCookie = () => {
    const name = "token"
    const date = new Date();
    date.setTime(date.getTime() + (2 * 60 * 60 * 1000));
    cookies.set(name, token, { expires: date, path: '/', secure: true, sameSite: 'strict' });
};


export function isAuthenticated() {
  cookies.get("token");
  return !!token;
}

export function getNameFromToken() {
    
    const tokenData = decodeToken(token);
    if (tokenData && tokenData.hasOwnProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")) {
        return tokenData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
    }
    return null;
}