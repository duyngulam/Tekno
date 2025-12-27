import { getBlogsList, getBlogsRecent } from "@/services/blogs";
import React from "react";
import BlogCard from "./BlogCard";
import { Blog } from "@/type/blog";

export default async function NewBlogs() {
  const data = await getBlogsList();

  const blogs: Blog[] = data.data.data.slice(0, 4);

  return (
    <div className="flex flex-col">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-10">
        {blogs?.map((blog) => (
          <BlogCard blog={blog} type="vertical" key={blog.id} />
        ))}
      </div>
    </div>
  );
}
