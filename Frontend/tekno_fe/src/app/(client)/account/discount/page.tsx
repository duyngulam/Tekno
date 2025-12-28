"use client";

import TitleAccount from "@/components/account/TitleAccount";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Eye } from "lucide-react";
import React, { useEffect, useMemo, useState } from "react";
import { getCoupons } from "@/services/coupon";
import type { Coupon } from "@/type/coupon";

export default function page() {
  const [search, setSearch] = useState<string>("");
  const [page, setPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);
  const [loading, setLoading] = useState<boolean>(false);
  const [coupons, setCoupons] = useState<Coupon[]>([]);
  const [totalRecords, setTotalRecords] = useState<number>(0);

  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        setLoading(true);
        const res = await getCoupons(search, page, pageSize);
        const data = res?.data;
        if (!mounted) return;
        setCoupons(data?.data ?? []);
        setTotalRecords(Number(data?.totalRecords ?? 0));
      } catch (e) {
        console.error("Fetch coupons error:", e);
        if (mounted) {
          setCoupons([]);
          setTotalRecords(0);
        }
      } finally {
        if (mounted) setLoading(false);
      }
    })();
    return () => {
      mounted = false;
    };
  }, [search, page, pageSize]);

  const totalPages = useMemo(
    () => Math.max(1, Math.ceil((totalRecords || 0) / pageSize)),
    [totalRecords, pageSize]
  );
  const canPrev = page > 1;
  const canNext = page < totalPages;

  return (
    <div className="flex flex-col gap-4">
      <TitleAccount
        title="Discounts & Voucher"
        des="Add discount code to apply a discount in your purchase"
      />

      {/* Search input */}
      <InputGroup>
        <InputGroupInput
          placeholder="Search your coupons code or name…"
          value={search}
          onChange={(e) => {
            setPage(1);
            setSearch(e.target.value);
          }}
        />
        <InputGroupAddon align="inline-end">
          <Eye />
        </InputGroupAddon>
      </InputGroup>

      {/* List coupons */}
      {loading ? (
        <div className="text-sm text-gray-500">Loading…</div>
      ) : coupons.length === 0 ? (
        <div className="text-sm text-gray-500">No coupons found</div>
      ) : (
        <Accordion type="multiple" className="space-y-2">
          {coupons.map((c) => (
            <AccordionItem
              key={c.id}
              value={String(c.id)}
              className="rounded-lg border bg-white px-4"
            >
              <AccordionTrigger className="py-3">
                <div className="flex items-center justify-between w-full">
                  <div className="flex flex-col">
                    <span className="text-sm font-semibold">{c.code}</span>
                    <span className="text-xs text-gray-500">
                      {c.name ?? ""}
                    </span>
                  </div>
                  <div className="text-xs text-gray-600">
                    {c.type === "Percentage"
                      ? `${c.value}%`
                      : `${c.value?.toLocaleString()} VNĐ`}
                  </div>
                </div>
              </AccordionTrigger>
              <AccordionContent className="py-3 text-sm text-gray-700">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-2">
                  <div>
                    <div className="text-xs text-gray-500">Valid from</div>
                    <div>
                      {c.startDate
                        ? new Date(c.startDate).toLocaleDateString()
                        : "-"}
                    </div>
                  </div>
                  <div>
                    <div className="text-xs text-gray-500">Expires</div>
                    <div>
                      {c.endDate
                        ? new Date(c.endDate).toLocaleDateString()
                        : "-"}
                    </div>
                  </div>
                  <div>
                    <div className="text-xs text-gray-500">Min order</div>
                    <div>
                      {c.minPurchaseAmount != null
                        ? `${Number(c.minPurchaseAmount).toLocaleString()} VNĐ`
                        : "-"}
                    </div>
                  </div>
                  <div>
                    <div className="text-xs text-gray-500">Usage left</div>
                    <div>{c.remainingQuantity ?? "-"}</div>
                  </div>
                </div>
              </AccordionContent>
            </AccordionItem>
          ))}
        </Accordion>
      )}

      {/* Pagination */}
      <div className="mt-2 flex items-center justify-between">
        <div className="flex items-center gap-2 text-sm text-gray-600">
          <span>
            Page {page} of {totalPages}
          </span>
          <select
            value={pageSize}
            onChange={(e) => {
              setPage(1);
              setPageSize(Number(e.target.value));
            }}
            className="border rounded-md px-2 py-1 text-sm"
          >
            <option value={10}>10</option>
            <option value={20}>20</option>
            <option value={50}>50</option>
          </select>
          <span>per page</span>
        </div>

        <div className="flex items-center gap-2">
          <button
            type="button"
            disabled={!canPrev}
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            className="px-3 py-1 rounded-md border text-sm disabled:opacity-50"
          >
            Prev
          </button>
          <button
            type="button"
            disabled={!canNext}
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
            className="px-3 py-1 rounded-md border text-sm disabled:opacity-50"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
}
