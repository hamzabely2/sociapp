import React, { useEffect, useState } from 'react';
import axios from 'axios';
import Cookies from "universal-cookie";
import 'react-toastify/dist/ReactToastify.css';
const cookies = new Cookies();

export async function CreatePost(payload) {
  
 
}
  

export async function UpdateListPostUser() {
    return await axios.get(`${process.env.REACT_APP_URL}post/get-all-user-posts`, {
        headers: {
          Authorization: `Bearer ${cookies.get("token")}`,
        },
      });
}
