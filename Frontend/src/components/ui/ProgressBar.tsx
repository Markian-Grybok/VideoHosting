import React from "react";

interface ProgressBarProps {
  value: number; // 0-100
  label?: string;
  status?: "processing" | "ready" | "failed";
  showPercent?: boolean;
}

const ProgressBar: React.FC<ProgressBarProps> = ({
  value,
  label,
  status = "processing",
  showPercent = true
}) => {
  const statusColors = {
    processing: "bg-primary-500",
    ready: "bg-green-500",
    failed: "bg-red-500"
  };

  const animateClass = status === "processing" ? "animate-pulse" : "";

  return (
    <div className="w-full">
      {(label || showPercent) && (
        <div className="flex justify-between items-center mb-2">
          {label && <span className="text-sm font-medium text-gray-700">{label}</span>}
          {showPercent && <span className="text-sm text-gray-500">{value}%</span>}
        </div>
      )}
      <div className="w-full bg-gray-200 rounded-full h-2">
        <div
          className={`h-2 rounded-full transition-all duration-500 ease-out ${statusColors[status]} ${animateClass}`}
          style={{ width: `${Math.min(Math.max(value, 0), 100)}%` }}
        />
      </div>
    </div>
  );
};

export default ProgressBar;
