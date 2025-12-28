"use client";

import React, { useEffect, useState } from "react";
import BlogCard from "./BlogCard";
import { Blog } from "@/type/blog";
import { getBlogsList } from "@/services/blogs";

export default function NewBlogs() {
  const [blogs, setBlogs] = useState<Blog[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  // pagination state
  const [page, setPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(4);
  const [totalRecords, setTotalRecords] = useState<number>(0);

  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        setLoading(true);
        const res = await getBlogsList(page, pageSize);
        // expected response: { data: Blog[], totalRecords, totalPages, ... }
        const list: Blog[] =
          (res?.data as any)?.data ??
          (res?.data as any) ??
          (res as any)?.data ??
          [];
        const total: number =
          (res as any)?.totalRecords ??
          (res?.data as any)?.totalRecords ??
          list.length;

        if (mounted) {
          setBlogs(list);
          setTotalRecords(Number(total || 0));
        }
      } catch (e) {
        console.error("Fetch blogs error:", e);
      } finally {
        if (mounted) setLoading(false);
      }
    })();
    return () => {
      mounted = false;
    };
  }, [page, pageSize]);

  const totalPages = Math.max(1, Math.ceil(totalRecords / pageSize));
  const canPrev = page > 1;
  const canNext = page < totalPages;

  return (
    <div className="flex flex-col gap-4">
      {loading ? (
        <div className="py-6 text-sm text-gray-500">Loading…</div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-10">
          {blogs?.map((blog) => (
            <BlogCard blog={blog} type="vertical" key={blog.id} />
          ))}
        </div>
      )}

      {/* Pagination */}
      <div className="flex items-center justify-between mt-2">
        <div className="flex items-center gap-2 text-sm text-gray-600">
          <span>
            Page {page} of {totalPages}
          </span>
          <select
            value={pageSize}
            onChange={(e) => {
              setPage(1);
              setPageSize(Number(e.target.value));
            }}
            className="border rounded-md px-2 py-1 text-sm"
          >
            <option value={4}>4</option>
            <option value={8}>8</option>
            <option value={12}>12</option>
          </select>
          <span>per page</span>
        </div>

        <div className="flex items-center gap-2">
          <button
            type="button"
            disabled={!canPrev}
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            className="px-3 py-1 rounded-md border text-sm disabled:opacity-50"
          >
            Prev
          </button>
          <button
            type="button"
            disabled={!canNext}
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
            className="px-3 py-1 rounded-md border text-sm disabled:opacity-50"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
}
