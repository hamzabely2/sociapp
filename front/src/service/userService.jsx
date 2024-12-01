import axios from 'axios';
import Cookies from "universal-cookie";

const cookies = new Cookies();

export const getUser = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}user/get-user`, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
      },
    });
    return response;
  } catch (error) {
    throw new Error("Erreur");
  }
};

export const getAllUsers = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}user/get-all-users`);
    return response; 
  } catch (error) {
    throw new Error("Erreur");
  }
}

export const updateUserProfile = async (userData) => {
  try {
    const response = await axios.put(
      `${process.env.REACT_APP_URL}user/update-user`, 
      userData, 
      {
        headers: {
          Authorization: `Bearer ${cookies.get("token")}`, 
        },
      }
    );
    return response;
  } catch (error) {
    throw new Error("Erreur"); 
  }
};



