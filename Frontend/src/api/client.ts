import axios from "axios";

const client = axios.create({
    baseURL: "", // через проксі Vite — порожній baseURL
    headers: { "Content-Type": "application/json" },
    timeout: 600000,
});

client.interceptors.response.use(
    (res) => res,
    (err) => {
        console.error("API error:", err.response?.data ?? err.message);
        return Promise.reject(err);
    }
);

export default client;