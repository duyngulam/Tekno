import { Product } from "./product"

export interface Blog {
        id: number,
        title: string,
        slug: string,
        summary: string,
        featuredImageUrl: string,
        authorName: string,
        status: string,
        viewCount: number,
        publishedAt: string,
        createdAt: string,
        tags: string[]
}

export interface BlogDetail{
        id: number,
    title: string,
    slug: string,
        summary: string,
        content: string,
        featuredImageUrl: string,
    authorName: string,
    status: string,
    viewCount: number,
    publishedAt: string,
    createdAt: string,
    updatedAt: string,
    tags: string[]
    relatedProducts: Product[]
}