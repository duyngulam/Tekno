import BlogCard from "@/components/blog/BlogCard";
import { Container } from "@/components/MainLayout/Container";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import React from "react";

export default function page() {
  return (
    <Container>
      <div className="col-span-12">
        <Breadcrumb />
      </div>

      <div className="col-span-8">
        <div>Blog posts</div>
        <div>
          Recent port
          <BlogCard />
        </div>
      </div>
      <div className="col-span-4">Videos</div>
      <div className="col-span-12 mx-auto">panigation</div>
    </Container>
  );
}
