import React, { useEffect, useState } from "react";
import { Disclosure, Menu } from "@headlessui/react";
import { BellIcon } from "@heroicons/react/24/outline";
import { Link , useNavigate  } from "react-router-dom";
import axios from "axios";
import Cookies from "universal-cookie";
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

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

 

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const response = await axios.get(
          `${process.env.REACT_APP_URL}user/get`,
          {
            headers: {
              Authorization: `Bearer ${cookies.get("token")}`,
            },
          }
        );
        setUser(response.data.respose);
      } catch (error) {
        toast.error(error);
      }
    };

    fetchUser();
  }, []);

  const handleDeconnexion = ( ) =>{
      cookies.remove('token');
      navigate('/login');
      window.location.reload();
  }
  
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
                              ? "border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center  text-sm font-medium"
                              : "border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700 inline-flex items-center  text-sm font-medium",
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
                    >
                      <span className="absolute -inset-1.5" />
                      <span className="sr-only">View notifications</span>
                      <BellIcon aria-hidden="true" className="h-6 w-6" />
                    </button>

                    {/* Profile dropdown */}
                    <Menu as="div" className="relative ml-3">
                      <div>
                        <Menu.Button className="relative flex max-w-xs items-center rounded-full bg-white text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2">
                          <span className="absolute -inset-1.5" />
                          <span className="sr-only">Open user menu</span>
                          <p className="text-gray-900">{user.userName}</p>
                        </Menu.Button>
                      </div>
                      <Menu.Items className="absolute right-0 z-10 mt-2 w-48 origin-top-right rounded-md bg-white py-1 shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none">
                        <Menu.Item >
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
                        <Menu.Item >
                          {({ active }) => (
                            <Link
                              to="/"
                              className={classNames(
                                active ? "bg-gray-100" : "",
                                "block px-4 py-2 text-sm text-gray-700"
                              )}
                            >
                              setting
                            </Link>
                          )}
                        </Menu.Item>
                        <Menu.Item >
                          {({ active }) => (
                            <button
                              onClick={handleDeconnexion}
                              className={classNames(
                                active ? "bg-gray-100" : "",
                                "block px-4 py-2 text-sm text-gray-700"
                              )}
                            >
                              Déconexion
                            </button>
                          )}
                        </Menu.Item>
                      </Menu.Items>
                    </Menu>
                  </div>
                </div>
              </div>

              <Disclosure.Panel className="sm:hidden">
                <div className="space-y-1 pb-3 pt-2">
                  {navigation.map((item) => (
                    <Disclosure.Button
                      key={item.name}
                      as={Link}
                      to={item.href}
                      aria-current={item.current ? "page" : undefined}
                      className={classNames(
                        item.current
                          ? "border-indigo-500 bg-indigo-50 text-indigo-700"
                          : "border-transparent text-gray-600 hover:border-gray-300 hover:bg-gray-50 hover:text-gray-800",
                        "block border-l-4 py-2 pl-3 pr-4 text-base font-medium"
                      )}
                    >
                      {item.name}
                    </Disclosure.Button>
                  ))}
                </div>     
              </Disclosure.Panel>
            </>
          )}
        </Disclosure>
      </div>
    </>
  );
}
