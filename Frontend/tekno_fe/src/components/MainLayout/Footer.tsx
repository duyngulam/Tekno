import React from "react";

export default function Footer() {
  return (
    <footer className="footer sm:footer-horizontal bg-secondary text-gray-100 p-12 lg:px-32">
      <nav>
        <h6 className="footer-title opacity-100">Company</h6>
        <a className="link link-hover">about us</a>
        <a className="link link-hover">blog</a>
        <a className="link link-hover">returns</a>
        <a className="link link-hover">order status</a>
      </nav>
      <nav>
        <h6 className="footer-title opacity-100 text-white">infor</h6>
        <a className="link link-hover">How it works?</a>
        <a className="link link-hover">our promise</a>
        <a className="link link-hover">FAQ</a>
      </nav>
      <nav>
        <h6 className="footer-title opacity-100 text-white">Contact us</h6>
        <a className="link link-hover">Terms of use</a>
        <a className="link link-hover">Privacy policy</a>
        <a className="link link-hover">Cookie policy</a>
      </nav>
      <form>
        <h6 className="footer-title opacity-100">
          Sign up for News and updates
        </h6>
        <label className="input bg-transparent validator">
          <svg
            className="h-[1em] opacity-50"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
          >
            <g
              strokeLinejoin="round"
              strokeLinecap="round"
              strokeWidth="2.5"
              fill="none"
              stroke="currentColor"
            >
              <rect width="20" height="16" x="2" y="4" rx="2"></rect>
              <path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7"></path>
            </g>
          </svg>
          <input type="email" placeholder="mail@site.com" required />
        </label>
        <div className="validator-hint hidden">Enter valid email address</div>
      </form>
    </footer>
  );
}
