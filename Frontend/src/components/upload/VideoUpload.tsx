import React, { useState, useRef } from "react";
import { Upload, X, FileVideo } from "lucide-react";
import Button from "../ui/Button";
import ProgressBar from "../ui/ProgressBar";
import { useFileUpload } from "../../hooks/useFileUpload";

interface VideoUploadProps {
    onUploaded: (fileId: string) => void;
}

const VideoUpload: React.FC<VideoUploadProps> = ({ onUploaded }) => {
    const [dragOver, setDragOver] = useState(false);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);
    const { upload, uploadProgress, uploading, error } = useFileUpload();

    const formatFileSize = (bytes: number): string => {
        if (bytes === 0) return "0 Б";
        const k = 1024;
        const sizes = ["Б", "КБ", "МБ", "ГБ"];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + " " + sizes[i];
    };

    const handleFileSelect = (file: File) => {
        setSelectedFile(file);
    };

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault();
        setDragOver(true);
    };

    const handleDragLeave = (e: React.DragEvent) => {
        e.preventDefault();
        setDragOver(false);
    };

    const handleDrop = (e: React.DragEvent) => {
        e.preventDefault();
        setDragOver(false);
        const file = e.dataTransfer.files[0];
        if (file && file.type.startsWith("video/")) {
            handleFileSelect(file);
        }
    };

    const handleFileInput = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            handleFileSelect(file);
        }
    };

    const handleUpload = async () => {
        if (!selectedFile) return;
        const fileId = await upload(selectedFile);
        if (fileId) {
            onUploaded(fileId);
        }
    };

    const handleClear = () => {
        setSelectedFile(null);
        if (fileInputRef.current) {
            fileInputRef.current.value = "";
        }
    };

    return (
        <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-4">Завантаження відео</h2>

            {!selectedFile ? (
                <div
                    className={`border-2 border-dashed rounded-xl p-12 text-center transition-colors ${
                        dragOver ? "border-primary-400 bg-primary-50" : "border-gray-300 bg-white"
                    }`}
                    onDragOver={handleDragOver}
                    onDragLeave={handleDragLeave}
                    onDrop={handleDrop}
                >
                    <Upload className="w-12 h-12 text-gray-400 mx-auto mb-4" />
                    <p className="text-gray-700 mb-2">Перетягніть відео сюди</p>
                    <p className="text-gray-400 text-sm mb-4">або</p>
                    <input
                        ref={fileInputRef}
                        type="file"
                        accept="video/mp4,video/quicktime,video/x-msvideo"
                        onChange={handleFileInput}
                        className="hidden"
                        id="video-upload"
                    />
                    <label htmlFor="video-upload">
                        <Button variant="secondary" size="md">
                            Вибрати файл
                        </Button>
                    </label>
                    <p className="text-xs text-gray-400 mt-4">
                        Підтримуються MP4, MOV, AVI до 500 МБ
                    </p>
                </div>
            ) : (
                <div className="space-y-4">
                    <div className="flex items-center gap-3 p-4 bg-gray-50 rounded-lg">
                        <FileVideo className="w-8 h-8 text-primary-500" />
                        <div className="flex-1 min-w-0">
                            <p className="text-sm font-medium text-gray-900 truncate">
                                {selectedFile.name}
                            </p>
                            <p className="text-xs text-gray-500">
                                {formatFileSize(selectedFile.size)}
                            </p>
                        </div>
                        <Button
                            variant="ghost"
                            size="sm"
                            onClick={handleClear}
                            disabled={uploading}
                        >
                            <X className="w-4 h-4" />
                        </Button>
                    </div>

                    {uploading && (
                        <div className="space-y-2">
                            <ProgressBar
                                value={uploadProgress}
                                label="Завантаження на сервер..."
                                status="processing"
                            />
                        </div>
                    )}

                    {error && (
                        <div className="p-3 bg-red-50 border border-red-200 rounded-lg">
                            <p className="text-sm text-red-600">{error}</p>
                        </div>
                    )}

                    <Button
                        variant="primary"
                        size="lg"
                        className="w-full"
                        onClick={handleUpload}
                        loading={uploading}
                        disabled={uploading}
                    >
                        {uploading ? "Завантаження..." : "Завантажити"}
                    </Button>
                </div>
            )}
        </div>
    );
};

export default VideoUpload;