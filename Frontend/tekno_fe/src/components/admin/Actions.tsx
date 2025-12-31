import { Pencil, Trash } from "lucide-react";
import { Button } from "@/components/ui/button";

export default function Actions({ onEdit, onDelete }: any) {
  return (
    <div className="flex gap-2">
      <Button
        variant="ghost"
        size="icon"
        data-testid="edit-button"
        onClick={(e) => {
          e.stopPropagation();   // ⛔ Chặn click nổi lên <tr>
          onEdit();
        }}
        className="text-blue-600 hover:text-blue-800"
      >
        <Pencil className="w-4 h-4" />
      </Button>

      <Button
        variant="ghost"
        size="icon"
        data-testid="delete-button" 
        onClick={(e) => {
          e.stopPropagation();   // ⛔ Chặn click nổi lên <tr>
          onDelete();
        }}
        className="text-red-600 hover:text-red-800"
      >
        <Trash className="w-4 h-4" />
      </Button>
    </div>
  );
}
