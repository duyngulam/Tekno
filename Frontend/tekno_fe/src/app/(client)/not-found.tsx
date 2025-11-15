import Link from "next/link";
import React from "react";

export default function NotFoundPage() {
  return (
    <div className="bg-white flex flex-col items-center justify-center px-4 sm:px-6 lg:px-8 py-10 md:py-32">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <p>Tekno</p>
          <h2 className="mt-6 text-3xl font-extrabold text-gray-900">
            Looking for something?
          </h2>
          <p className="mt-2 text-sm text-gray-600">
            We are sorry. The web address your entered is not a functioning page
            on our site
          </p>
        </div>
        <div className="mt-8 space-y-6">
          <div className="rounded-md shadow-sm space-y-4">
            <Link
              href={"/"}
              className="w-full flex items-center justify-center px-4 py-2 border border-transparent text-sm font-semibold rounded-md text-white bg-secondary/80 hover:bg-secondary focus:outline-none focus:ring-2 focus:ring-offset-2 focus::ring-blue hoverEffect"
            >
              Go back Home
            </Link>
            <Link
              href={"/"}
              className="w-full flex items-center justify-center px-4 py-2 border border-gray-300 text-sm font-semibold rounded-md text-blue bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus::ring-blue"
            >
              Help
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
