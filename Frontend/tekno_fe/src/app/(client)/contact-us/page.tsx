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

      <div className="col-span-12 mx-auto">Contract</div>

      <div className="col-span-6">
        <div>Message us</div>
        <div>
          We're here to assist you every step of the way. Whether you have a
          question, need technical support, or simply want to share your
          feedback, our dedicated team is ready to listen and provide prompt
          assistance.
        </div>
      </div>
      <div className="col-span-6">Videos</div>
    </Container>
  );
}
