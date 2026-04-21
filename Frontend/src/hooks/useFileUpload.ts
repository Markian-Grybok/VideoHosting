import { useState } from "react";
import { fileApi } from "../api/fileApi";

export function useFileUpload() {
  const [uploadProgress, setUploadProgress] = useState(0);
  const [fileId, setFileId] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const upload = async (file: File) => {
    setUploading(true);
    setError(null);
    setUploadProgress(0);
    try {
      const result = await fileApi.uploadVideo(file, setUploadProgress);
      setFileId(result.fileId);
      return result.fileId;
    } catch {
      setError("Помилка завантаження файлу");
      return null;
    } finally {
      setUploading(false);
    }
  };

  return { upload, uploadProgress, fileId, uploading, error };
}
