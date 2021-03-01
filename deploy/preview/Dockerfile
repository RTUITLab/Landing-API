FROM nginx:1.19.3-alpine

RUN mkdir /app
COPY preview-build/wwwroot /app

COPY nginx.conf /etc/nginx/nginx.conf
