import React, { useEffect, useState } from "react";
import { Disclosure, Menu, Dialog } from "@headlessui/react";
import { BellIcon } from "@heroicons/react/24/outline";
import { Link, useNavigate } from "react-router-dom";
import Cookies from "universal-cookie";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { fetchNotifications, deleteNotification } from "../../service/notificationService";
import { getUser, updateUserProfile } from '../../service/userService'; // Ajout de updateUserProfile
const navigation = [
  { name: "Post", href: "/", current: true },
  { name: "Utilisateur", href: "/user", current: false },
  { name: "Follow", href: "/follow", current: false },
];

function classNames(...classes) {
  return classes.filter(Boolean).join(" ");
}

export default function NavBar() {
  const cookies = new Cookies();
  const [user, setUser] = useState([]);
  const navigate = useNavigate();
  const [notifications, setNotifications] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSettingsModalOpen, setIsSettingsModalOpen] = useState(false);  // Etat du modal de paramètres
  const [isProfilePrivate, setIsProfilePrivate] = useState(false); // Etat de la confidentialité du profil

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const data = await getUser(); 
        setUser(data.data.response);
        setIsProfilePrivate(data.data.response.isProfilePrivate);  // Initialiser avec l'état actuel du profil
      } catch (error) {
        toast.error("Erreur lors de la récupération de l'utilisateur.");
      }
    };

    fetchUser();
  }, []);

  const handleDeconnexion = () => {
    cookies.remove("token");
    navigate("/login");
    window.location.reload();
  };

  const loadNotifications = async () => {
    try {
      const data = await fetchNotifications(); 
      setNotifications(data.data.response);
    } catch (error) {
      toast.error(error.message);
    }
  };

  const handleDeleteNotification = async (id) => {
    try {
      await deleteNotification(id); 
      toast.success("Notification supprimée !");
      setNotifications((prev) =>
        prev.filter((notification) => notification.id !== id)
      );
    } catch (error) {
      toast.error(error.message);
    }
  };

  const handleSaveSettings = async () => {
    try {
      // Appel à la fonction du backend pour mettre à jour les paramètres du profil
      await updateUserProfile({ isProfilePrivate });
      toast.success("Paramètres enregistrés !");
      setIsSettingsModalOpen(false);
    } catch (error) {
      toast.error("Erreur lors de la mise à jour des paramètres.");
    }
  };

  return (
    <>
      <div className="min-h-full">
        <Disclosure as="nav" className="border-b border-gray-200 bg-white">
          {({ open }) => (
            <>
              <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
                <div className="flex h-16 justify-between">
                  <div className="flex">
                    <div className="flex shrink-0 items-center">
                      <img
                        className="mx-auto h-10 w-auto"
                        src="/img/logo.png"
                        alt="Pot Shop"
                      />
                    </div>
                    <div className="hidden sm:-my-px sm:ml-6 sm:flex sm:space-x-8">
                      {navigation.map((item) => (
                        <Link
                          key={item.name}
                          to={item.href}
                          aria-current={item.current ? "page" : undefined}
                          className={classNames(
                            item.current
                              ? "border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center text-sm font-medium"
                              : "border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center text-sm font-medium"
                          )}
                        >
                          {item.name}
                        </Link>
                      ))}
                    </div>
                  </div>
                  <div className="hidden sm:ml-6 sm:flex sm:items-center">
                    <button
                      type="button"
                      className="relative rounded-full bg-white p-1 text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
                      onClick={() => {
                        loadNotifications();
                        setIsModalOpen(true);
                      }}
                    >
                      <span className="sr-only">View notifications</span>
                      <BellIcon aria-hidden="true" className="h-6 w-6" />
                    </button>

                    {/* Profile dropdown */}
                    <Menu as="div" className="relative ml-3">
                      <div>
                        <Menu.Button className="relative flex max-w-xs items-center rounded-full bg-white text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2">
                          <span className="sr-only">Open user menu</span>
                          <p className="text-gray-900">{user.userName}</p>
                        </Menu.Button>
                      </div>
                      <Menu.Items className="absolute right-0 z-10 mt-2 w-48 origin-top-right rounded-md bg-white py-1 shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none">
                        <Menu.Item>
                          {({ active }) => (
                            <Link
                              to="/postuser"
                              className={classNames(
                                active ? "bg-gray-100" : "",
                                "block px-4 py-2 text-sm text-gray-700"
                              )}
                            >
                              List Post
                            </Link>
                          )}
                        </Menu.Item>
                        <Menu.Item>
                          {({ active }) => (
                            <button
                              onClick={() => setIsSettingsModalOpen(true)} // Ouvre le modal des paramètres
                              className={classNames(
                                active ? "bg-gray-100" : "",
                                "block px-4 py-2 text-sm text-gray-700"
                              )}
                            >
                              Settings
                            </button>
                          )}
                        </Menu.Item>
                        <Menu.Item>
                          {({ active }) => (
                            <button
                              onClick={handleDeconnexion}
                              className={classNames(
                                active ? "bg-gray-100" : "",
                                "block px-4 py-2 text-sm text-gray-700"
                              )}
                            >
                              Déconnexion
                            </button>
                          )}
                        </Menu.Item>
                      </Menu.Items>
                    </Menu>
                  </div>
                </div>
              </div>
            </>
          )}
        </Disclosure>
      </div>

      {/* Modal for notifications */}
      <Dialog
        open={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        className="relative z-10"
      >
        <div className="fixed inset-0 bg-black bg-opacity-30" aria-hidden="true" />
        <div className="fixed inset-0 flex items-center justify-center p-4">
          <Dialog.Panel className="w-full max-w-md transform overflow-hidden rounded-2xl bg-white p-6 shadow-xl transition-all">
            <Dialog.Title className="text-lg font-medium leading-6 text-gray-900">
              Notifications
            </Dialog.Title>
            <div className="mt-4">
              {notifications.length > 0 ? (
                notifications.map((notification) => (
                  <div
                    key={notification.id}
                    className="flex items-center justify-between p-2 border-b"
                  >
                    <p>{notification.message}</p>
                    <button
                      onClick={() => handleDeleteNotification(notification.id)}
                      className="text-red-600 hover:text-red-800"
                    >
                      Supprimer
                    </button>
                  </div>
                ))
              ) : (
                <p className="text-gray-500">Aucune notification.</p>
              )}
            </div>
            <button
              onClick={() => setIsModalOpen(false)}
              className="mt-4 w-full inline-flex justify-center rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
            >
              Fermer
            </button>
          </Dialog.Panel>
        </div>
      </Dialog>

      {/* Modal for settings */}
      <Dialog
        open={isSettingsModalOpen}
        onClose={() => setIsSettingsModalOpen(false)}
        className="relative z-10"
      >
        <div className="fixed inset-0 bg-black bg-opacity-30" aria-hidden="true" />
        <div className="fixed inset-0 flex items-center justify-center p-4">
          <Dialog.Panel className="w-full max-w-md transform overflow-hidden rounded-2xl bg-white p-6 shadow-xl transition-all">
            <Dialog.Title className="text-lg font-medium leading-6 text-gray-900">
              Paramètres du Profil
            </Dialog.Title>
            <div className="mt-4">
              <div className="flex items-center justify-between">
                <p>Profil privé</p>
                <input
                  type="checkbox"
                  checked={isProfilePrivate}
                  onChange={() => setIsProfilePrivate(!isProfilePrivate)} // Changer l'état du profil privé
                  className="h-5 w-5"
                />
              </div>
            </div>
            <button
              onClick={handleSaveSettings} // Enregistrer les paramètres
              className="mt-4 w-full inline-flex justify-center rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
            >
              Sauvegarder
            </button>
            <button
              onClick={() => setIsSettingsModalOpen(false)} // Fermer le modal
              className="mt-2 w-full inline-flex justify-center rounded-md bg-gray-300 px-4 py-2 text-sm font-medium text-gray-800 hover:bg-gray-400"
            >
              Annuler
            </button>
          </Dialog.Panel>
        </div>
      </Dialog>
    </>
  );
}
