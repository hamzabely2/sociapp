import axios from 'axios';
import Cookies from 'universal-cookie';


export const getAllUsers = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}user/get-all-users`);
    return response; 
  } catch (error) {
    
    throw error; 
  }
};


