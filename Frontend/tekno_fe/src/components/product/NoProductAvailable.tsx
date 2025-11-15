import { cn } from "@/lib/utils";
import React from "react";
import { motion } from "motion/react";
import { Loader2 } from "lucide-react";

export default function NoProductAvailable({
  selectedCategory,
  className,
}: {
  selectedCategory?: string;
  className?: string;
}) {
  return (
    <div
      className={cn(
        "flex flex-col items-center justify-center py-10 min-h-80 space-y-4 text-centre bg-gray-100 rounded-lg w-full mt-10",
        className
      )}
    >
      <motion.div
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <h2 className="text-bold font-bold text-gray-800">
          No Product Available
        </h2>
      </motion.div>
      <motion.p
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ delay: 0.2, duration: 0.5 }}
        className="text-gray-600"
      >
        We are sorry, but there are no product matching on {""}
        <span className="text-base font-semibold text-blue-950">
          {selectedCategory}
        </span>{" "}
        criteria at moment.
      </motion.p>
      <motion.div
        animate={{ scale: [1, 1.1, 1] }}
        transition={{ repeat: Infinity, duration: 1.5 }}
        className="flex items-center space-x-5 text-secondary "
      >
        <Loader2 className="h-5 w-5 animate-spin" />
        <span>We are stocking shortly</span>
      </motion.div>
      <motion.p
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ delay: 0.4, duration: 0.5 }}
        className="text-sm text-gray-500"
      >
        Please check back later or explore our other product categories.
      </motion.p>
    </div>
  );
}
