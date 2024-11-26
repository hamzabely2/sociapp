import { Disclosure } from '@headlessui/react'
import { BellIcon } from '@heroicons/react/24/outline'
import Post from './Post'
import { Link } from 'react-router-dom'


export default function Home() {
  let user = false
  return (
    <div className="min-h-full">
      <Disclosure as="nav" className="bg-gray-800">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="flex h-16 items-center justify-between">
            <div className="flex items-center">
              <div className="shrink-0">
              <Link to="/">
                <img
                  className="h-12 w-auto"
                  src="/img/logo.png"
                  alt="pot shop"
                />
                </Link>
              </div>   
              <Link to="/user">
                <button
                  type="button"
                  className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 m-2"
                >
                  Utilisateur
                </button>
              </Link>
              <div className="hidden md:block">
                <div className="ml-10 flex items-baseline space-x-4">

                </div>
              </div>
            </div>
            <div className="hidden md:block">
              <div className="ml-4 flex items-center md:ml-6">
                {!user ? (
                  <>
                    <Link to="/login">
                      <button
                        type="button"
                        className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 m-2"
                      >
                        Login
                      </button>
                    </Link>
                    <Link to="/register">
                      <button
                        type="button"
                        className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
                      >
                        Register
                      </button>
                    </Link>
                  </>
                ) : (
                  <button
                    type="button"
                    className="relative rounded-full bg-gray-800 p-1 text-gray-400 hover:text-white focus:outline-none focus:ring-2 focus:ring-white focus:ring-offset-2 focus:ring-offset-gray-800"
                  >
                    <span className="absolute -inset-1.5" />
                    <span className="sr-only">View notifications</span>
                    <BellIcon aria-hidden="true" className="h-6 w-6" />
                  </button>
                )}

              </div>
            </div>
          </div>
        </div>
      </Disclosure>
      <main>
        <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8"><Post /></div>
      </main>
    </div>
  )
}
