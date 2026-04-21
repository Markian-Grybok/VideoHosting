import React, { useRef, useEffect } from "react";
import Hls from "hls.js";

interface VideoPlayerProps {
    src: string;
}

const VideoPlayer: React.FC<VideoPlayerProps> = ({ src }) => {
    const videoRef = useRef<HTMLVideoElement>(null);

    useEffect(() => {
        const video = videoRef.current;
        if (!video) return;

        let hls: Hls | null = null;

        if (Hls.isSupported()) {
            hls = new Hls();
            hls.loadSource(src);
            hls.attachMedia(video);
        } else if (video.canPlayType("application/vnd.apple.mpegurl")) {
            video.src = src;
        }

        return () => {
            if (hls) {
                hls.destroy(); // ← очищення HLS інстансу
            }
        };
    }, [src]);

    return (
        <video
            ref={videoRef}
            controls
            className="w-full rounded-xl bg-black aspect-video"
            playsInline
        />
    );
};

export default VideoPlayer;