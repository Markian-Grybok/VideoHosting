import React, { useState } from "react";
import { useCourses, useDeleteCourse } from "../hooks/useCourses";
import Layout from "../components/layout/Layout";
import Button from "../components/ui/Button";
import Card, { CardHeader, CardBody } from "../components/ui/Card";
import CourseCard from "../components/courses/CourseCard";
import CourseForm from "../components/courses/CourseForm";
import type { Course } from "../types";

const CoursesPage: React.FC = () => {
    const { data: coursesData, isLoading, error } = useCourses();
    const deleteCourseMutation = useDeleteCourse();
    const [showCreateModal, setShowCreateModal] = useState(false);

    // Адаптивне отримання масиву курсів
    const getCoursesArray = (): Course[] => {
        if (!coursesData) return [];

        // Якщо coursesData — це масив
        if (Array.isArray(coursesData)) return coursesData;

        // Якщо coursesData має поле items
        if ('items' in coursesData && Array.isArray(coursesData.items)) {
            return coursesData.items;
        }

        console.warn("Невідомий формат даних:", coursesData);
        return [];
    };

    const courses = getCoursesArray();

    const handleDeleteCourse = async (courseId: string) => {
        if (window.confirm("Ви впевнені, що хочете видалити цей курс?")) {
            await deleteCourseMutation.mutateAsync(courseId);
        }
    };

    const handleCreateSuccess = () => {
        setShowCreateModal(false);
    };

    const SkeletonCard = () => (
        <Card className="animate-pulse">
            <CardBody className="p-6">
                <div className="h-6 bg-gray-200 rounded mb-2"></div>
                <div className="h-4 bg-gray-200 rounded mb-1"></div>
                <div className="h-4 bg-gray-200 rounded mb-4"></div>
                <div className="flex justify-between items-center">
                    <div className="h-4 bg-gray-200 rounded w-16"></div>
                    <div className="h-6 bg-gray-200 rounded w-6"></div>
                </div>
            </CardBody>
        </Card>
    );

    if (error) {
        return (
            <Layout>
                <div className="text-center py-12">
                    <div className="text-6xl mb-4">❌</div>
                    <h3 className="text-lg font-medium text-gray-900 mb-2">
                        Помилка завантаження курсів
                    </h3>
                    <p className="text-gray-500">{String(error)}</p>
                    <Button onClick={() => window.location.reload()} className="mt-4">
                        Спробувати знову
                    </Button>
                </div>
            </Layout>
        );
    }

    return (
        <Layout>
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold">Курси</h1>
                <Button onClick={() => setShowCreateModal(true)}>
                    Створити курс
                </Button>
            </div>

            {isLoading ? (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 mt-6">
                    {Array.from({ length: 6 }).map((_, i) => (
                        <SkeletonCard key={i} />
                    ))}
                </div>
            ) : courses.length === 0 ? (
                <div className="text-center py-12">
                    <div className="text-6xl mb-4">📚</div>
                    <h3 className="text-lg font-medium text-gray-900 mb-2">
                        Курсів ще немає
                    </h3>
                    <Button onClick={() => setShowCreateModal(true)}>
                        Створити перший курс
                    </Button>
                </div>
            ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 mt-6">
                    {courses.map((course: Course) => (
                        <CourseCard
                            key={course.id}
                            course={course}
                            onDelete={handleDeleteCourse}
                        />
                    ))}
                </div>
            )}

            {showCreateModal && (
                <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
                    <Card className="max-w-lg w-full">
                        <CardHeader>
                            <h2 className="text-lg font-semibold">Новий курс</h2>
                        </CardHeader>
                        <CardBody>
                            <CourseForm
                                onSuccess={handleCreateSuccess}
                                onCancel={() => setShowCreateModal(false)}
                            />
                        </CardBody>
                    </Card>
                </div>
            )}
        </Layout>
    );
};

export default CoursesPage;