// src/services/categories.ts
import { httpClient } from "@/lib/httpClient";
import { Category, CategoryAttribute } from "@/type/categories";

export interface AttributeValue {
  id: number;
  attributeId: number;
  value: string;
}

export interface AttributeValuesResponse {
  id: number;
  name: string;
  inputType: string;
  isGlobal: boolean;
  categoryId: number | null;
  categoryName: string | null;
  values: AttributeValue[];
}

export interface CreateAttributeRequest {
  name: string;
  inputType: string;
  isGlobal: boolean;
  categoryId: number;
  initialValues: string[];
}

export class CategoryService {
  private static instance: CategoryService | null = null;

  private constructor() {}

  public static getInstance(): CategoryService {
    if (!CategoryService.instance) {
      CategoryService.instance = new CategoryService();
    }
    return CategoryService.instance;
  }

  public async getCategoriesList(): Promise<Category[]> {
    return httpClient.get<Category[]>("/admin/categories/list", {
      cache: "no-store",
    });
  }

  public async getCategoriesTree(): Promise<Category[]> {
    return httpClient.get<Category[]>("/categories/tree", {
      cache: "no-store",
    });
  }

  public async createCategory(fd: FormData) {
    return httpClient.post<any>("/admin/categories/create", fd);
  }

  public async updateCategory(fd: FormData) {
    return httpClient.put<any>("/admin/categories/update", fd);
  }

  public async deleteCategory(id: number) {
    return httpClient.del<any>("/admin/categories/delete", {
      body: JSON.stringify({ Id: id }),
    });
  }

  public async getCategoryAttributesForFilter(
    categorySlug: string
  ): Promise<CategoryAttribute[]> {
    return httpClient.get<CategoryAttribute[]>(
      `/categories/${categorySlug}/attributes`,
      { cache: "no-store" }
    );
  }

  public async getCategoryAttributes(categoryId: number) {
    return httpClient.get<any>(
      `/admin/categories/${categoryId}/attributes`,
      { cache: "no-store" }
    );
  }

  public async createCategoryAttribute(
    categoryId: number,
    name: string,
    inputType: string = "text"
  ) {
    return httpClient.post<any>("/admin/categories/attributes", {
      categoryId,
      name,
    });
  }

  public async updateCategoryAttribute(
    attributeId: number,
    name: string,
    inputType: string = "text"
  ) {
    return httpClient.put<any>(
      `/admin/categories/attributes/${attributeId}`,
      { name }
    );
  }

  public async deleteCategoryAttribute(attributeId: number) {
    return httpClient.del<any>(
      `/admin/categories/attributes/${attributeId}`,
      {
        body: JSON.stringify({ AttributeId: attributeId }),
      }
    );
  }

  public async getCategoryAttributeValues(
    attributeId: number
  ): Promise<AttributeValuesResponse> {
    return httpClient.get<AttributeValuesResponse>(
      `/admin/categories/attributes/${attributeId}`,
      { cache: "no-store" }
    );
  }

  public async addCategoryAttributeValue(attributeId: number, value: string) {
    const body = { AttributeId: attributeId, Value: value };
    return httpClient.post<any>("/admin/categories/attributes/values", body);
  }

  public async deleteCategoryAttributeValue(valueId: number, value: string) {
    const body = { ValueId: valueId, Value: value };
    return httpClient.del<any>(
      `/admin/categories/attributes/values/${valueId}`,
      { body: JSON.stringify(body) }
    );
  }

  public async updateCategoryAttributeValues(
    valueId: number,
    values: string[]
  ) {
    const body = { ValueId: valueId, Values: values };
    return httpClient.put<any>(
      `/admin/categories/attributes/values/${valueId}`,
      body
    );
  }

  public async createAttribute(data: CreateAttributeRequest) {
    return httpClient.post<any>("/admin/categories/attributes", data);
  }

  public async getGlobalAttributes() {
    return httpClient.get<any>("/admin/categories/attributes/global", {
      cache: "no-store",
    });
  }
}

export const categoryService = CategoryService.getInstance();

// Backward compatibility exports
export const getCategoriesList = () => categoryService.getCategoriesList();
export const getCategoriesTree = () => categoryService.getCategoriesTree();
export const createCategory = (fd: FormData) => categoryService.createCategory(fd);
export const updateCategory = (fd: FormData) => categoryService.updateCategory(fd);
export const deleteCategory = (id: number) => categoryService.deleteCategory(id);
export const getCategoryAttributesForFilter = (categorySlug: string) => categoryService.getCategoryAttributesForFilter(categorySlug);
export const getCategoryAttributes = (categoryId: number) => categoryService.getCategoryAttributes(categoryId);
export const createCategoryAttribute = (categoryId: number, name: string, inputType?: string) => categoryService.createCategoryAttribute(categoryId, name, inputType);
export const updateCategoryAttribute = (attributeId: number, name: string, inputType?: string) => categoryService.updateCategoryAttribute(attributeId, name, inputType);
export const deleteCategoryAttribute = (attributeId: number) => categoryService.deleteCategoryAttribute(attributeId);
export const getCategoryAttributeValues = (attributeId: number) => categoryService.getCategoryAttributeValues(attributeId);
export const addCategoryAttributeValue = (attributeId: number, value: string) => categoryService.addCategoryAttributeValue(attributeId, value);
export const deleteCategoryAttributeValue = (valueId: number, value: string) => categoryService.deleteCategoryAttributeValue(valueId, value);
export const updateCategoryAttributeValues = (valueId: number, values: string[]) => categoryService.updateCategoryAttributeValues(valueId, values);
export const createAttribute = (data: CreateAttributeRequest) => categoryService.createAttribute(data);
export const getGlobalAttributes = () => categoryService.getGlobalAttributes();