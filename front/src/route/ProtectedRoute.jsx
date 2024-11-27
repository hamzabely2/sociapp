import { Navigate } from 'react-router-dom';
import Cookies from 'universal-cookie';
const cookies = new Cookies();

function isAuthenticated() {
    let token = cookies.get('token');
    if(token){
      return true;
    }else{
      return false;
    }
  }

export function ProtectedRoute({ children }) {

  if (!isAuthenticated())  {
    return <Navigate to="/login" replace />;
  }

  return children;
}






