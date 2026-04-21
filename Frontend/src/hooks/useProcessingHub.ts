import { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";

interface ProcessingUpdate {
    fileId: string;
    status: string;
    progressPercent: number;
}

export function useProcessingHub(fileId: string | null) {
    const [processingStatus, setProcessingStatus] = useState<string>("Pending");
    const [processingProgress, setProcessingProgress] = useState(0);

    useEffect(() => {
        if (!fileId) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/processing") // ← відносний URL через проксі
            .withAutomaticReconnect()
            .build();

        connection.on("ProcessingUpdate", (update: ProcessingUpdate) => {
            if (update.fileId === fileId) {
                setProcessingStatus(update.status);
                setProcessingProgress(update.progressPercent);
            }
        });

        connection.start()
            .then(() => connection.invoke("SubscribeToFile", fileId))
            .catch(err => console.error("SignalR error:", err));

        return () => {
            connection.stop(); // ← очищення підключення
        };
    }, [fileId]);

    return { processingStatus, processingProgress };
}