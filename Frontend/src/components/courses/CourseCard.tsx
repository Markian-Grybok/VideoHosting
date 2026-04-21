import React from "react";
import { useNavigate } from "react-router-dom";
import { Trash2 } from "lucide-react";
import Card, { CardBody } from "../ui/Card";
import type { Course } from "../../types";

interface CourseCardProps {
  course: Course;
  onDelete: (id: string) => void;
}

const CourseCard: React.FC<CourseCardProps> = ({ course, onDelete }) => {
  const navigate = useNavigate();

  const handleDelete = (e: React.MouseEvent) => {
    e.stopPropagation();
    onDelete(course.id);
  };

  return (
    <Card
      hover
      onClick={() => navigate(`/courses/${course.id}`)}
      className="cursor-pointer"
    >
      <CardBody className="p-6">
        <div className="flex items-start justify-between mb-4">
          <div className="text-2xl">🎓</div>
          <button
            onClick={handleDelete}
            className="text-gray-400 hover:text-red-500 transition-colors p-1"
          >
            <Trash2 size={16} />
          </button>
        </div>

        <h3 className="text-lg font-semibold text-gray-900 mb-2 line-clamp-2">
          {course.title}
        </h3>

        <p className="text-sm text-gray-500 mb-4 line-clamp-3">
          {course.description}
        </p>

        <div className="flex items-center justify-between">
          <span className="text-xs text-gray-400">
            {course.lessonCount} уроків
          </span>
        </div>
      </CardBody>
    </Card>
  );
};

export default CourseCard;
