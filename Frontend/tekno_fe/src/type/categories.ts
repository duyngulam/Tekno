export interface Category {
  id: number;
  name: string;
  slug: string;
  iconPath: string;
  imageUrl: string;
  parentId: number | null;
  description: string
}

