const axios = require('axios');

export const getAllUsers = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}user/get-all-users`);
    return response; 
  } catch (error) {
    throw error; 
  }
};


