import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from './route/ProtectedRoute';
import Cookies from 'universal-cookie';
import NoPage from '../src/page/components/NoPage';
import Login from './page/connexion/Login';
import Register from './page/connexion/Register';
import {  ToastContainer } from 'react-toastify';
import FollowList from './page/userPage/FollowList';
import PostList from './page/publicPage/PostList';
import PostUser from './page/userPage/PostUser';
import UserList from './page/publicPage/UserList';

const cookies = new Cookies();

function App() {
  function isAuthenticated() {
    let token = cookies.get('token');
    if(token){
      return true;
    }else{
      return false;
    }
  }
   

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={!isAuthenticated() === false ? <Navigate to="/" replace /> : <Login />} />
        <Route path="/register" element={!isAuthenticated() === false ? <Navigate to="/" replace /> : <Register />} />
        <Route
          path="/user"
          element={
            <ProtectedRoute>
              <UserList />
            </ProtectedRoute>
          }
        />
         <Route
          path="/postuser"
          element={
            <ProtectedRoute>
              <PostUser />
            </ProtectedRoute>
          }
        />
        <Route
          path="/follow"
          element={
            <ProtectedRoute>
              <FollowList />
            </ProtectedRoute>
          }
        />
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <PostList />
            </ProtectedRoute>
          }
        />

        <Route path="*" element={<NoPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
