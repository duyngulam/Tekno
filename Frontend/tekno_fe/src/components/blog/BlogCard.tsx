import { Blog } from "@/type/blog";
import { Calendar } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import React from "react";

export default function BlogCard({
  blog,
  type,
}: {
  blog: Blog;
  type?: "horizontal" | "vertical";
}) {
  return (
    <Link href={`/blogs/${blog.slug}`} className="flex space-x-4 w-fit group">
      <div
        className={`flex  ${
          type === "horizontal" ? "flex-row" : "flex-col"
        }  gap-4 `}
      >
        <Image
          src={blog.featuredImageUrl}
          alt={blog.title}
          width={200}
          height={80}
          className={` object-cover rounded-md w-full ${
            type === "vertical" ? " h-55" : "max-w-60 h-auto min-w-30"
          }`}
        ></Image>
        <div
          className={`flex flex-col   ${
            type === "horizontal" ? "my-auto" : ""
          }`}
        >
          <div className="text-sm text-gray-700 ">
            <Calendar className="inline-block mr-1 mb-1" size={14} />
            {new Date(blog.createdAt).toLocaleString("vi-VN")}
          </div>
          <div className="font-semibold text-lg line-clamp-1 group-hover:text-secondary hoverEffect">
            {blog.title}
          </div>
          <div className="text-sm text-gray-500 line-clamp-3">
            {blog.summary}
          </div>
          <div className="text-secondary bottom-0 opacity-0 group-hover:opacity-100 hoverEffect text-end">
            Read more
          </div>
        </div>
      </div>
    </Link>
  );
}
