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