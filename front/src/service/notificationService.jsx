import axios from "axios";
import Cookies from "universal-cookie";

const cookies = new Cookies();

export const fetchNotifications = async () => {
  try {
    const response = await axios.get(`${process.env.REACT_APP_URL}notification/get-notifications`, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
      },
    });
    return response;
  } catch (error) {
    throw new Error("Erreur lors de la récupération des notifications.");
  }
};

export const deleteNotification = async (id) => {
  try {
    await axios.delete(`${process.env.REACT_APP_URL}notification/delete-notification/${id}`, {
      headers: {
        Authorization: `Bearer ${cookies.get("token")}`,
      },
    });
  } catch (error) {
    throw new Error("Erreur lors de la suppression de la notification.");
  }
};
