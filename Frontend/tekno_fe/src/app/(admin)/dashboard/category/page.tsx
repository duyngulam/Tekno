"use client";

import React, { useEffect, useMemo, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { get, postForm } from "@/lib/api";
import { ChevronDown, ChevronRight } from "lucide-react";
import  Actions  from "@/components/admin/Actions";


type CategoryNode = {
  id: number | string;
  name: string;
  parentId?: number | string | null;
  children?: CategoryNode[];
  iconPath?: string;
  imageUrl?: string;
  [k: string]: any;
};

export default function CategoryPage() {
  
  const assignParentIds = (nodes: any[], parentId: number | string | null = null) => {
  return nodes.map(node => {
    node.parentId = parentId;

    if (node.subCategories && node.subCategories.length > 0) {
      node.subCategories = assignParentIds(node.subCategories, node.id);
    }

    return node;
  });
  };
  
  const [tree, setTree] = useState<CategoryNode[]>([]);
  const [loading, setLoading] = useState(true);

  const [expandedIds, setExpandedIds] = useState<Set<number | string>>(
    new Set()
  );

  const [openCreate, setOpenCreate] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);

  const [deleteLoading, setDeleteLoading] = useState(false);

  const [form, setForm] = useState({
    name: "",
    parentId: "",
    iconFile: null as File | null,
    imageFile: null as File | null,
  });

  const [createData, setCreateData] = useState({
    name: "",
    slug: "",
    parentId: "",
    iconFile: null as File | null,
    imageFile: null as File | null,
  });

  const [editData, setEditData] = useState<any>({
    id: "",
    name: "",
    parentId: "",
    iconFile: null,
    imageFile: null,
  });

  // Load Tree
  useEffect(() => {
    loadTree();
  }, []);

const loadTree = async () => {
  try {
    const json = await get("http://localhost:5000/api/admin/categories/tree");

    let list = Array.isArray(json?.data?.data)
      ? json.data.data
      : Array.isArray(json?.data)
      ? json.data
      : json;

    // ⭐ FIX: tự bổ sung ParentId
    list = assignParentIds(list);

    setTree(list);

  } catch (e) {
    console.error(e);
    setTree([]);
  } finally {
    setLoading(false);
  }
};


  // Flatten for Select Options
  const flat = useMemo(() => {
    const out: any[] = [];

    const walk = (nodes: any[], depth = 0) => {
      for (const n of nodes) {
        out.push({ ...n, depth });
        const children =
          n.children ??
          (n.subCategories ?? []);

        if (children?.length) walk(children, depth + 1);
      }
    };

    walk(tree);
    return out;
  }, [tree]);

  // Expand handler
  const toggleExpanded = (id: number | string) => {
    setExpandedIds((prev) => {
      const s = new Set(prev);
      s.has(id) ? s.delete(id) : s.add(id);
      return s;
    });
  };

  // ------------ CREATE CATEGORY ------------
const handleCreate = async () => {
  try {
    if (!createData.name || !createData.slug) {
      alert("Name và Slug là bắt buộc");
      return;
    }

    const fd = new FormData();

    fd.append("Name", createData.name);
    fd.append("Slug", createData.slug); // ⭐ Required

    if (createData.parentId)
      fd.append("ParentId", String(createData.parentId));

    if (createData.iconFile)
      fd.append("IconFile", createData.iconFile);

    if (createData.imageFile)
      fd.append("ImageFile", createData.imageFile);

    const res = await fetch(
      "http://localhost:5000/api/admin/categories/create",
      {
        method: "POST",
        body: fd,
      }
    );

    if (!res.ok) {
      const text = await res.text();
      throw new Error(`Create failed: ${text}`);
    }

    // Reload tree view
    await loadTree();

    // Reset form
    setCreateData({
      name: "",
      slug: "",
      parentId: "",
      iconFile: null,
      imageFile: null,
    });

    setOpenCreate(false);
    alert("Category created successfully!");

  } catch (err) {
    console.error(err);
    alert("Create failed");
  }
};


  // ------------ EDIT CATEGORY ------------
  const openEdit = (cat: any) => {
  setEditData({
    id: cat.id,
    name: cat.name,
    slug: cat.slug,  
    parentId:
      cat.parentId ??
      cat.parentID ??
      (cat.parent?.id ?? ""),
    iconPath: cat.iconPath ?? null,
    imageUrl: cat.imageUrl ?? null,
    iconFile: null,
    imageFile: null,
  });
  setOpenEditDialog(true);
  };


const handleEditSave = async () => {
  try {
    const fd = new FormData();

    fd.append("Id", String(editData.id));
    fd.append("Name", editData.name);
    fd.append("Slug", editData.slug);
    fd.append("IsActive", String(editData.isActive));

    if (editData.parentId)
      fd.append("ParentId", String(editData.parentId));

    if (editData.iconFile)
      fd.append("IconFile", editData.iconFile);

    if (editData.imageFile)
      fd.append("ImageFile", editData.imageFile);

    const res = await fetch("http://localhost:5000/api/admin/categories/update", {
      method: "PUT",
      body: fd,
    });

    if (!res.ok) {
      const text = await res.text();
      throw new Error(`Update failed: ${text}`);
    }

    await loadTree();
    setOpenEditDialog(false);
    alert("Category updated");

  } catch (err) {
    console.error(err);
    alert("Update failed");
  }
};



  // ------------ DELETE CATEGORY ------------
  const deleteCategory = async (id: number | string) => {
    if (!confirm("Delete category?")) return;

    try {
      setDeleteLoading(true);

      const res = await fetch(
        `http://localhost:5000/api/admin/categories/${id}`,
        { method: "DELETE" }
      );

      if (!res.ok) throw new Error("Delete failed");

      await loadTree();
      alert("Category deleted");
    } catch (err) {
      console.error(err);
      alert("Delete failed");
    } finally {
      setDeleteLoading(false);
    }
  };

  // ------------ RENDER TREE (RECURSIVE) ------------
  const renderNode = (node: CategoryNode, depth = 0): any => {
    const children =
      node.children ??
      (node as any).subCategories ??
      [];

    const hasChildren = children.length > 0;
    const isExpanded = expandedIds.has(node.id);

    return (
      <React.Fragment key={node.id}>
        <tr className="border-b hover:bg-gray-50">
          <td
            className="p-2 cursor-pointer"
            onClick={() => hasChildren && toggleExpanded(node.id)}
          >
            <div className="flex items-center">
              <div style={{ width: depth * 20 }}></div>

              {hasChildren ? (
                isExpanded ? (
                  <ChevronDown className="w-4 h-4" />
                ) : (
                  <ChevronRight className="w-4 h-4" />
                )
              ) : (
                <div className="w-4 h-4"></div>
              )}
            </div>
          </td>

          <td className="p-2">{node.id}</td>

          <td className="p-2">
            {node.iconPath && (
              <img src={node.iconPath} className="w-6 h-6 object-contain" />
            )}
          </td>

          <td className="p-2">
            {node.imageUrl && (
              <img
                src={node.imageUrl}
                className="w-20 h-12 rounded object-cover"
              />
            )}
          </td>

          <td className="p-2">{node.name}</td>
          <td className="p-2">{children.length}</td>

          <td className="p-2">
            <Actions
              onEdit={() => openEdit(node)}
              onDelete={() => deleteCategory(node.id)}
            />
          </td>
        </tr>

        {isExpanded &&
          hasChildren &&
          children.map((child: any) => renderNode(child, depth + 1))}
      </React.Fragment>
    );
  };

  const flatten = (nodes: any[], depth = 0) => {
  let list: any[] = [];

  for (const n of nodes) {
    list.push({
      id: n.id,
      name: `${"— ".repeat(depth)}${n.name}`, // indent để nhìn rõ cấp
    });

    if (n.subCategories?.length > 0) {
      list = list.concat(flatten(n.subCategories, depth + 1));
    }
  }

  return list;
};

const flatCategories = flatten(tree);

  return (
    <div className="p-6">
      <div className="flex justify-between mb-4">
        <h2 className="text-xl font-semibold">Categories</h2>

        <Button onClick={() => setOpenCreate(true)}>+ Create</Button>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-sm bg-white shadow rounded">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2 w-12"></th>
                <th className="p-2">ID</th>
                <th>Icon</th>
                <th>Image</th>
                <th>Name</th>
                <th>Children</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>{tree.map((root) => renderNode(root))}</tbody>
          </table>
        </div>
      )}

      {/* ------------ CREATE MODAL ------------ */}
      <Dialog open={openCreate} onOpenChange={setOpenCreate}>
  <DialogContent>
    <DialogHeader>
      <DialogTitle>Create Category</DialogTitle>
    </DialogHeader>

    <div className="space-y-4">

      {/* Name */}
      <div>
        <label className="text-sm font-medium">Name *</label>
        <Input
          value={createData.name}
          onChange={(e) =>
            setCreateData({ ...createData, name: e.target.value })
          }
        />
      </div>

      {/* Slug */}
      <div>
        <label className="text-sm font-medium">Slug *</label>
        <Input
          value={createData.slug}
          onChange={(e) =>
            setCreateData({ ...createData, slug: e.target.value })
          }
        />
      </div>

      {/* Parent */}
      <div>
        <label className="text-sm font-medium">Parent category</label>
        <select
          className="w-full border p-2 rounded"
          value={createData.parentId}
          onChange={(e) =>
            setCreateData({ ...createData, parentId: e.target.value })
          }
        >
          <option value="">(No parent)</option>

          {flatCategories.map((cat) => (
            <option key={cat.id} value={cat.id}>
              {cat.name}
            </option>
          ))}
        </select>
      </div>

      {/* IconFile */}
      <div>
        <label className="text-sm font-medium">Icon File</label>
        <input
          type="file"
          accept="image/*"
          className="border p-2 rounded w-full"
          onChange={(e) =>
            setCreateData({
              ...createData,
              iconFile: e.target.files?.[0] || null,
            })
          }
        />
      </div>

      {/* ImageFile */}
      <div>
        <label className="text-sm font-medium">Image File</label>
        <input
          type="file"
          accept="image/*"
          className="border p-2 rounded w-full"
          onChange={(e) =>
            setCreateData({
              ...createData,
              imageFile: e.target.files?.[0] || null,
            })
          }
        />
      </div>
    </div>

    <div>
      <Button onClick={handleCreate}>Create</Button>
    </div>
  </DialogContent>
</Dialog>


      {/* ------------ EDIT MODAL ------------ */}
      <Dialog open={openEditDialog} onOpenChange={setOpenEditDialog}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Edit Category</DialogTitle>
          </DialogHeader>

          {editData && (
  <div className="grid gap-3 mt-2">

    {/* Name */}
    <div>
      <label className="text-sm font-medium">Name *</label>
      <Input
        value={editData.name}
        onChange={(e) =>
          setEditData({ ...editData, name: e.target.value })
        }
      />
    </div>
    
    <div>
    <label className="text-sm font-medium">Slug *</label>
      <Input
        value={editData.slug}
        onChange={(e) =>
        setEditData({ ...editData, slug: e.target.value })
      }
      />
    </div>


    {/* Parent */}
    <div>
      <label className="text-sm font-medium">Parent</label>
      <select
        className="border p-2 rounded w-full"
        value={editData.parentId}
        onChange={(e) =>
          setEditData({ ...editData, parentId: e.target.value })
        }
      >
        <option value="">-- None --</option>
        {flat.map((f) => (
          <option key={f.id} value={String(f.id)}>
            {"—".repeat(f.depth)} {f.name}
          </option>
        ))}
      </select>
    </div>

{/* IconFile */}
<div>
  <label className="text-sm font-medium">Icon File</label>
  <input
    type="file"
    accept="image/*"
    className="border p-2 rounded w-full"
    onChange={(e) =>
      setEditData({
        ...editData,
        iconFile: e.target.files?.[0] || null,
      })
    }
  />

  {/* icon preview */}
  {editData.iconPath && !editData.iconFile && (
    <img
      src={editData.iconPath}
      className="w-10 h-10 object-contain border rounded mt-2"
      alt="Old Icon"
    />
  )}
</div>

{/* ImageFile */}
<div>
  <label className="text-sm font-medium">Image File</label>
  <input
    type="file"
    accept="image/*"
    className="border p-2 rounded w-full"
    onChange={(e) =>
      setEditData({
        ...editData,
        imageFile: e.target.files?.[0] || null,
      })
    }
  />

  {/* image preview */}
  {editData.imageUrl && !editData.imageFile && (
    <img
      src={editData.imageUrl}
      className="w-28 h-20 object-cover rounded border mt-2"
      alt="Old Image"
    />
  )}
</div>


    <Button onClick={handleEditSave}>Save</Button>
  </div>
)}

        </DialogContent>
      </Dialog>
    </div>
  );
}
