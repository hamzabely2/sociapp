import './App.css';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from './page/Home';
import Login from './page/connexion/Login';
import Register from './page/connexion/Register';
import NoPage from './page/NoPage';
import UserList from './page/UsersList';


function App() {
  return (
    <BrowserRouter>
    <Routes>
        <Route index element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/user" element={<UserList />} />
        <Route path="*" element={<NoPage />} />
    </Routes>
  </BrowserRouter>
  );
}

export default App;
