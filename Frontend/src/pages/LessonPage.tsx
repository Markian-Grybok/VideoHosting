import React from "react";
import { useParams, useNavigate } from "react-router-dom";
import { ArrowLeft, Upload, Film, Loader2, AlertCircle } from "lucide-react";
import Layout from "../components/layout/Layout";
import Card, { CardBody } from "../components/ui/Card";
import Button from "../components/ui/Button";
import Badge from "../components/ui/Badge";
import VideoPlayer from "../components/lessons/VideoPlayer";
import { useLesson } from "../hooks/useLessons"; // ← виправлений шлях

const LessonPage: React.FC = () => {
    const { courseId, lessonId } = useParams<{ courseId: string; lessonId: string }>();
    const navigate = useNavigate();

    const { data: lesson, isLoading, error } = useLesson(lessonId!);

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString("uk-UA", {
            year: "numeric",
            month: "long",
            day: "numeric",
        });
    };

    if (isLoading) {
        return (
            <Layout>
                <div className="animate-pulse">
                    <div className="h-8 w-48 bg-gray-200 rounded mb-6" />
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                        <div className="lg:col-span-2">
                            <div className="bg-gray-200 rounded-xl aspect-video" />
                        </div>
                        <div className="space-y-4">
                            <div className="h-6 w-24 bg-gray-200 rounded" />
                            <div className="h-8 w-full bg-gray-200 rounded" />
                            <div className="h-20 w-full bg-gray-200 rounded" />
                        </div>
                    </div>
                </div>
            </Layout>
        );
    }

    if (error || !lesson) {
        return (
            <Layout>
                <div className="text-center py-12">
                    <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <AlertCircle className="w-8 h-8 text-red-600" />
                    </div>
                    <h2 className="text-xl font-semibold text-gray-900 mb-2">Урок не знайдено</h2>
                    <p className="text-gray-500 mb-6">
                        Можливо, урок було видалено або ви перейшли за неправильним посиланням
                    </p>
                    <Button variant="primary" onClick={() => navigate(`/courses/${courseId}`)}>
                        ← Повернутися до курсу
                    </Button>
                </div>
            </Layout>
        );
    }

    const videoStatus = lesson.videoStatus;
    const playbackUrl = lesson.playbackUrl;
    const hasVideo = !!lesson.videoFileId;

    const renderVideoContent = () => {
        if (videoStatus === "Ready" && playbackUrl) {
            return <VideoPlayer src={playbackUrl} />;
        }

        if (videoStatus === "Processing") {
            return (
                <div className="bg-gray-50 rounded-xl border border-gray-200 p-12 text-center">
                    <div className="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <Loader2 className="w-8 h-8 text-blue-600 animate-spin" />
                    </div>
                    <h3 className="text-lg font-semibold text-gray-900 mb-2">Відео обробляється</h3>
                    <p className="text-gray-500">
                        Будь ласка, зачекайте. Це може зайняти кілька хвилин.
                    </p>
                </div>
            );
        }

        if (videoStatus === "Failed") {
            return (
                <div className="bg-red-50 rounded-xl border border-red-200 p-12 text-center">
                    <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <AlertCircle className="w-8 h-8 text-red-600" />
                    </div>
                    <h3 className="text-lg font-semibold text-red-700 mb-2">
                        Помилка обробки відео
                    </h3>
                    <p className="text-red-600 mb-4">
                        Не вдалося обробити відео. Спробуйте завантажити його знову.
                    </p>
                    <Button variant="primary" onClick={() => navigate("/upload")}>
                        Завантажити нове відео
                    </Button>
                </div>
            );
        }

        if (!hasVideo) {
            return (
                <div className="bg-gray-50 rounded-xl border-2 border-dashed border-gray-200 p-12 text-center">
                    <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <Film className="w-8 h-8 text-gray-400" />
                    </div>
                    <h3 className="text-lg font-semibold text-gray-900 mb-2">
                        Відео ще не завантажено
                    </h3>
                    <p className="text-gray-500 mb-4">Додайте відео до цього уроку</p>
                    <Button variant="primary" onClick={() => navigate("/upload")}>
                        <Upload className="w-4 h-4 mr-2" />
                        Завантажити відео
                    </Button>
                </div>
            );
        }

        return null;
    };

    return (
        <Layout>
            {/* Back button */}
            <button
                onClick={() => navigate(`/courses/${courseId}`)}
                className="flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-6 transition-colors"
            >
                <ArrowLeft className="w-4 h-4" />
                ← Урок {lesson.order}: {lesson.title}
            </button>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Left column - Video player */}
                <div className="lg:col-span-2">{renderVideoContent()}</div>

                {/* Right column - Lesson info */}
                <div className="lg:col-span-1">
                    <Card>
                        <CardBody>
                            <div className="mb-4">
                                <span className="text-sm text-gray-500">Урок {lesson.order}</span>
                                <h1 className="text-2xl font-bold text-gray-900 mt-1">
                                    {lesson.title}
                                </h1>
                            </div>

                            <p className="text-gray-600 leading-relaxed mb-6">{lesson.description}</p>

                            <hr className="border-gray-100 my-4" />

                            <div className="space-y-3">
                                <div className="flex justify-between items-center">
                                    <span className="text-sm text-gray-500">Статус відео</span>
                                    <Badge status={videoStatus || "Pending"} />
                                </div>

                                {lesson.videoFileId && (
                                    <div className="flex justify-between items-center">
                                        <span className="text-sm text-gray-500">ID файлу</span>
                                        <span className="text-xs font-mono text-gray-400">
                      {lesson.videoFileId.slice(0, 8)}...
                    </span>
                                    </div>
                                )}

                                <div className="flex justify-between items-center">
                                    <span className="text-sm text-gray-500">Створено</span>
                                    <span className="text-sm text-gray-600">
                    {formatDate(lesson.createdAt)}
                  </span>
                                </div>

                                {lesson.updatedAt !== lesson.createdAt && (
                                    <div className="flex justify-between items-center">
                                        <span className="text-sm text-gray-500">Оновлено</span>
                                        <span className="text-sm text-gray-600">
                      {formatDate(lesson.updatedAt)}
                    </span>
                                    </div>
                                )}
                            </div>
                        </CardBody>
                    </Card>
                </div>
            </div>
        </Layout>
    );
};

export default LessonPage;