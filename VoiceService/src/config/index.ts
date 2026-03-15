import dotenv from "dotenv";
dotenv.config();

interface Config {
  port: number;
  corsAllowedOrigins: string[];
  jwt: {
    key: string;
    issuer: string;
    audience: string;
  };
  livekit: {
    apiKey: string;
    apiSecret: string;
    url: string;
  };
}

function requireEnv(name: string): string {
  const value = process.env[name];
  if (!value) {
    throw new Error(`Missing required environment variable: ${name}`);
  }
  return value;
}

const config: Config = {
  port: parseInt(process.env.PORT ?? "4000", 10),
  corsAllowedOrigins: (process.env.CORS_ALLOWED_ORIGINS
    ?? "http://localhost:5000,https://voxify.com.tr,https://www.voxify.com.tr")
    .split(",")
    .map((origin) => origin.trim())
    .filter((origin) => origin.length > 0),
  jwt: {
    key: requireEnv("JWT_KEY"),
    issuer: requireEnv("JWT_ISSUER"),
    audience: requireEnv("JWT_AUDIENCE"),
  },
  livekit: {
    apiKey: requireEnv("LIVEKIT_API_KEY"),
    apiSecret: requireEnv("LIVEKIT_API_SECRET"),
    url: requireEnv("LIVEKIT_URL"),
  },
};

export default config;
