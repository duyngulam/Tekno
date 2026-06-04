// src/services/blogs.ts
import { httpClient } from "@/lib/httpClient";
import { Blog, BlogDetail } from "@/type/blog";

export class BlogService {
  private static instance: BlogService | null = null;

  private constructor() {}

  public static getInstance(): BlogService {
    if (!BlogService.instance) {
      BlogService.instance = new BlogService();
    }
    return BlogService.instance;
  }

  // Client side
  public async getBlogsRecent(count: number): Promise<Blog[]> {
    return httpClient.get<Blog[]>(`/blog/recent?count=${count}`, {
      cache: "no-store",
    });
  }

  public async getBlogsList(page: number = 1, pageSize: number = 12) {
    const params = new URLSearchParams({
      page: String(page),
      pageSize: String(pageSize),
    });
    return httpClient.get<any>(`/blog?${params.toString()}`, {
      cache: "no-store",
    });
  }

  public async getBlogDetail(slug: string): Promise<BlogDetail> {
    return httpClient.get<BlogDetail>(`/blog/${slug}`, {
      cache: "no-store",
    });
  }

  // Admin side
  public async getAdminBlogs() {
    return httpClient.get<any>("/admin/blog", { cache: "no-store" });
  }

  public async getAdminBlog(id: number | string) {
    return httpClient.get<any>(`/admin/blog/${id}`, { cache: "no-store" });
  }

  public async createAdminBlog(fd: FormData) {
    return httpClient.post<any>("/admin/blog", fd);
  }

  public async updateAdminBlog(id: number | string, fd: FormData) {
    return httpClient.put<any>(`/admin/blog/${id}`, fd);
  }

  public async deleteAdminBlog(id: number | string) {
    return httpClient.del<any>(`/admin/blog/${id}`);
  }

  public async publishBlog(id: number | string) {
    return httpClient.patch<any>(`/admin/blog/${id}/publish`, {});
  }

  public async unpublishBlog(id: number | string) {
    return httpClient.patch<any>(`/admin/blog/${id}/unpublish`, {});
  }
}

export const blogService = BlogService.getInstance();

// Backward compatibility exports
export const getBlogsRecent = (count: number) => blogService.getBlogsRecent(count);
export const getBlogsList = (page: number = 1, pageSize: number = 12) => blogService.getBlogsList(page, pageSize);
export const getBlogDetail = (slug: string) => blogService.getBlogDetail(slug);
export const getAdminBlogs = () => blogService.getAdminBlogs();
export const getAdminBlog = (id: number | string) => blogService.getAdminBlog(id);
export const createAdminBlog = (fd: FormData) => blogService.createAdminBlog(fd);
export const updateAdminBlog = (id: number | string, fd: FormData) => blogService.updateAdminBlog(id, fd);
export const deleteAdminBlog = (id: number | string) => blogService.deleteAdminBlog(id);
export const publishBlog = (id: number | string) => blogService.publishBlog(id);
export const unpublishBlog = (id: number | string) => blogService.unpublishBlog(id);