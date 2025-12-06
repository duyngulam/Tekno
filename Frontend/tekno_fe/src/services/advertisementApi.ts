export const API_BASE = "http://localhost:5000/api/admin/advertisements";

export const advertisementApi = {
  // GET ALL
  async getAll() {
    const res = await fetch(API_BASE, { cache: "no-store" });
    return res.json();
  },

  // GET BY ID
  async getById(id: string) {
    const res = await fetch(`${API_BASE}/${id}`);
    return res.json();
  },

  // CREATE
  async create(data: any) {
    const res = await fetch(API_BASE, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    return res.json();
  },

  // UPDATE
  async update(id: string, data: any) {
    const res = await fetch(`${API_BASE}/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    return res.json();
  },

  // DELETE
  async delete(id: string) {
    return fetch(`${API_BASE}/${id}`, { method: "DELETE" });
  },

  // ACTIVATE
  async activate(id: string) {
    return fetch(`${API_BASE}/${id}/activate`, { method: "PATCH" });
  },

  // DEACTIVATE
  async deactivate(id: string) {
    return fetch(`${API_BASE}/${id}/deactivate`, { method: "PATCH" });
  },
};