import { getBlogsRecent } from "@/services/blogs";
import React from "react";
import BlogCard from "./BlogCard";
import Title from "../share/Title";

export default async function RecentReports() {
  const blogs = await getBlogsRecent(4); // await getBlogsRecent();

  return (
    <div className="flex flex-col">
      <Title title="Recent Reports" />

      <div className="flex flex-col gap-2">
        {blogs?.map((blog) => (
          <BlogCard blog={blog} type="horizontal" key={blog.id} />
        ))}
      </div>
    </div>
  );
}
