FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY publish/wwwroot/ /usr/share/nginx/html/

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80