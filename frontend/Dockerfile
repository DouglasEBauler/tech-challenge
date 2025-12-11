FROM node:20-alpine AS builder
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install

COPY . .
RUN npm run build -- --configuration production

FROM nginx:alpine AS final

COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=builder /app/dist/tech-challenge/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]