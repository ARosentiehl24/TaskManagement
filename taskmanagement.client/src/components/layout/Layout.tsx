import React from "react";
import { Container } from "react-bootstrap";
import Header from "./Header";

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div className="min-vh-100 bg-light">
      <Header />
      <main>
        <Container className="py-4">{children}</Container>
      </main>
    </div>
  );
};

export default Layout;
