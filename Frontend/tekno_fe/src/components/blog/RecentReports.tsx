import { getBlogsRecent } from "@/services/blogs";
import React from "react";
import BlogCard from "./BlogCard";

export default async function RecentReports() {
  const blogs = await getBlogsRecent(); // await getBlogsRecent();

  return (
    <div className="flex flex-col">
      <div className="text-2xl font-bold mb-4">Recent Reports</div>
      <div className="flex flex-col gap-2">
        {blogs?.map((blog) => (
          <BlogCard blog={blog} type="horizontal" key={blog.id} />
        ))}
      </div>
    </div>
  );
}
