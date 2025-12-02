import BlogCard from "@/components/blog/BlogCard";
import NewBlogs from "@/components/blog/NewBlogs";
import RecentReports from "@/components/blog/RecentReports";
import { Container } from "@/components/MainLayout/Container";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { getBlogsList, getBlogsRecent } from "@/services/blogs";
import React from "react";

export default async function page() {
  return (
    <Container className="flex flex-col space-y-5 my-10">
      <Breadcrumb />

      <div className="w-full flex space-x-5">
        <div className="w-3/4 gap-10 flex flex-col">
          <NewBlogs />
          <RecentReports />
        </div>
        <div className="w-1/3">Videos</div>
      </div>

      <div className=" mx-auto">panigation</div>
    </Container>
  );
}
