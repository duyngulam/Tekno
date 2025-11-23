import type { NextConfig } from "next";

const nextConfig: NextConfig = {
images: {
    domains: ["res.cloudinary.com"], // host ảnh cũ của bạn
    remotePatterns: [
      {
        protocol: "https",
        hostname: "i.pinimg.com",
        port: "",
        pathname: "/**",
      },
    ],
  },
};

export default nextConfig;



