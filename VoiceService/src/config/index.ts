import dotenv from "dotenv";
dotenv.config();

interface Config {
  port: number;
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
  livekit: {
    apiKey: requireEnv("LIVEKIT_API_KEY"),
    apiSecret: requireEnv("LIVEKIT_API_SECRET"),
    url: requireEnv("LIVEKIT_URL"),
  },
};

export default config;
