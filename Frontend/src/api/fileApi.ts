import client from "./client";
import type { UploadResult, FileStatus } from "../types";
import { AxiosError } from "axios";

export const fileApi = {
    uploadVideo: async (file: File, onProgress?: (percent: number) => void): Promise<UploadResult> => {
        const formData = new FormData();
        formData.append("file", file);

        // Діагностика: перевіряємо, що відправляється
        console.log("Uploading file:", {
            name: file.name,
            size: file.size,
            type: file.type,
        });

        // Перевіряємо вміст FormData
        for (const pair of formData.entries()) {
            console.log("FormData entry:", pair[0], pair[1]);
        }

        try {
            const response = await client.post<UploadResult>("/api/files/upload", formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
                onUploadProgress: (progressEvent) => {
                    if (onProgress && progressEvent.total) {
                        const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total);
                        onProgress(percent);
                    }
                },
            });
            console.log("Upload success:", response.data);
            return response.data;
        } catch (error: unknown) {
            if (error instanceof AxiosError) {
                console.error("Upload error details:", {
                    status: error.response?.status,
                    data: error.response?.data,
                    headers: error.response?.headers,
                });
            } else {
                console.error("Upload error:", error);
            }
            throw error;
        }
    },

    getStatus: async (fileId: string): Promise<FileStatus> => {
        const response = await client.get<FileStatus>(`/api/files/${fileId}/status`);
        return response.data;
    },
};