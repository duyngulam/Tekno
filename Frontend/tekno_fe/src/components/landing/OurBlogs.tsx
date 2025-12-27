"use client";
import { getProductsList } from "@/services/products";
import { Product } from "@/type/product";
import { ChevronRight } from "lucide-react";
import React, { useEffect, useState } from "react";
import ProductCard from "../product/ProductCard";
import { useRouter } from "next/navigation";
import { Blog } from "@/type/blog";
import BlogCard from "../blog/BlogCard";
import { getBlogsList } from "@/services/blogs";

const data = await getBlogsList();

const blogs: Blog[] = data.data.data.slice(0, 4);
export default function OurBlogs() {
  const router = useRouter();
  // const [blogs, setBlogs] = useState<Blog[]>();

  return (
    <div className="flex flex-col gap-5">
      <div className="border-b border-gray-500 flex items-center justify-between pb-2">
        <div className="font-semibold text-2xl">Our Blogs</div>
        <button
          className="flex items-center gap-2 hoverEffect mx-10 hover:cursor-pointer"
          onClick={() => router.push("/blogs")}
        >
          View all <ChevronRight className="w-5 h-5" />
        </button>
      </div>
      <div className="grid grid-col-2 md:grid-cols-4 gap-5">
        {blogs &&
          blogs
            ?.slice(0, 4)
            .map((blog) => (
              <BlogCard type="vertical" blog={blog} key={blog.id}></BlogCard>
            ))}
      </div>
    </div>
  );
}
