export interface Category {
  iconPath: string;
  id: number;
  name: string;
  parentId: number | null;
  slug: string;
}