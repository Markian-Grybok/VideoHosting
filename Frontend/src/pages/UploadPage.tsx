import React, { useState } from "react";
import Layout from "../components/layout/Layout";
import VideoUpload from "../components/upload/VideoUpload";
import UploadProgress from "../components/upload/UploadProgress";

const UploadPage: React.FC = () => {
    const [uploadedFileId, setUploadedFileId] = useState<string | null>(null);

    return (
        <Layout>
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Завантаження відео</h1>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                <VideoUpload onUploaded={setUploadedFileId} />
                <UploadProgress fileId={uploadedFileId} />
            </div>
        </Layout>
    );
};

export default UploadPage;