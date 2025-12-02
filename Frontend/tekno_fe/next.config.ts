import type { NextConfig } from "next";

const nextConfig: NextConfig = {
images: {
  remotePatterns: [
    {
      protocol: "https",
      hostname: "**", // cho phép mọi hostname ⚠️
    },
  ],
},

};


export default nextConfig;