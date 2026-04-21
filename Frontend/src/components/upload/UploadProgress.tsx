import React from "react";
import { CheckCircle, XCircle, Circle, Loader2 } from "lucide-react";
import Badge from "../ui/Badge";
import ProgressBar from "../ui/ProgressBar";
import Button from "../ui/Button";
import { useProcessingHub } from "../../hooks/useProcessingHub";

interface UploadProgressProps {
    fileId: string | null;
}

const UploadProgress: React.FC<UploadProgressProps> = ({ fileId }) => {
    const { processingStatus, processingProgress } = useProcessingHub(fileId);

    const getProgressLabel = (progress: number): string => {
        if (progress <= 5) return "Підготовка...";
        if (progress <= 30) return "Завантаження файлу...";
        if (progress <= 60) return "Конвертація FFmpeg...";
        if (progress <= 90) return "Збереження сегментів HLS...";
        return "Завершення...";
    };

    const getStatusForProgress = (status: string): "processing" | "ready" | "failed" => {
        if (status === "Ready") return "ready";
        if (status === "Failed") return "failed";
        return "processing";
    };

    const steps = [
        { key: "uploaded", label: "Файл завантажено на сервер", threshold: 5 },
        { key: "converted", label: "Конвертація у HLS формат", threshold: 60 },
        { key: "saved", label: "Збереження у сховище", threshold: 90 },
        { key: "ready", label: "Готово до перегляду", threshold: 100 },
    ];

    const isStepDone = (threshold: number): boolean => {
        if (processingStatus === "Ready") return true;
        if (processingStatus === "Failed") return false;
        return processingProgress > threshold;
    };

    if (!fileId) {
        return (
            <div className="bg-white rounded-xl border-2 border-dashed border-gray-200 p-12 text-center">
                <Loader2 className="w-12 h-12 text-gray-300 mx-auto mb-4 animate-spin" />
                <p className="text-gray-500">Тут з'явиться прогрес обробки відео</p>
            </div>
        );
    }

    if (processingStatus === "Ready") {
        return (
            <div className="bg-white rounded-xl border border-green-200 shadow-sm p-8 text-center">
                <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <CheckCircle className="w-10 h-10 text-green-600" />
                </div>
                <h3 className="text-xl font-semibold text-green-700 mb-2">
                    Відео готове до перегляду!
                </h3>
                <p className="text-xs text-gray-400 font-mono mb-4 break-all">{fileId}</p>
                <Button variant="primary" onClick={() => window.location.href = "/courses"}>
                    Прив'язати до уроку
                </Button>
            </div>
        );
    }

    if (processingStatus === "Failed") {
        return (
            <div className="bg-white rounded-xl border border-red-200 shadow-sm p-8 text-center">
                <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <XCircle className="w-10 h-10 text-red-600" />
                </div>
                <h3 className="text-xl font-semibold text-red-700 mb-4">
                    Помилка обробки відео
                </h3>
                <Button variant="primary" onClick={() => window.location.reload()}>
                    Спробувати знову
                </Button>
            </div>
        );
    }

    return (
        <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
            <div className="flex items-center justify-between mb-4">
                <div className="flex items-center gap-2">
                    <Badge status={processingStatus as "Pending" | "Processing" | "Ready" | "Failed"} />
                    <span className="font-semibold text-gray-900">Обробка відео</span>
                </div>
            </div>

            <ProgressBar
                value={processingProgress}
                label={getProgressLabel(processingProgress)}
                status={getStatusForProgress(processingStatus)}
                showPercent
            />

            <div className="mt-6 space-y-3">
                {steps.map((step) => (
                    <div key={step.key} className="flex items-center gap-3">
                        <div className="flex-shrink-0">
                            {isStepDone(step.threshold) ? (
                                <CheckCircle className="w-5 h-5 text-green-500" />
                            ) : (
                                <Circle className="w-5 h-5 text-gray-300" />
                            )}
                        </div>
                        <span
                            className={`text-sm ${
                                isStepDone(step.threshold) ? "text-gray-900" : "text-gray-400"
                            }`}
                        >
              {step.label}
            </span>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default UploadProgress;