<%@ Page Title="" Language="C#" MasterPageFile="~/JSpecs/JSpecsMaster.Master" AutoEventWireup="true"
    CodeBehind="JSpecsFAQ.aspx.cs" Inherits="JSpecs.Forms.JSpecsFAQ" %>
<%@ MasterType VirtualPath="~/JSpecs/JSpecsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="title">
        <h1 class="title__name title__name--FAQ"></h1>
    </div>

        <article class="FAQs">
            <div class="FAQ">
                <input class="FAQ__input" type="checkbox" id="faq1" />
                <label class="FAQ__question" for="faq1">
                    I don’t see an option to order the device I need. How do I order it if it’s not there?
                <div class="FAQ__svg-container">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 25 22">
                        <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z" />
                    </svg>
                </div>
                </label>
                <div class="FAQ__answer-box">
                    <p class="FAQ__answer">
                        Only things you can order in the app show up in your options. This is based on what devices you are authorized and your ordering history. If there is a device you need but can’t order through the application, please go to your local military Optometry Clinic.
                    </p>
                </div>
            </div>

            <div class="FAQ">
                <input class="FAQ__input" type="checkbox" id="faq2" />
                <label class="FAQ__question" for="faq2">
                    How long does it take to get my order?
                <div class="FAQ__svg-container">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 25 22">
                        <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z" />
                    </svg>
                </div>
                </label>
                <div class="FAQ__answer-box">
                    <p class="FAQ__answer">
                        The orders through JSPECS typically take longer than if you order through the clinic, unless you are deployed. We typically make the glasses in two weeks or less, and mailing can take another week to get to you. However, it could take more or less time, depending on operational demands at the laboratories. If you need your order quickly, you will most likely get them faster if you go to the clinic.
                    </p>
                </div>
            </div>

            <div class="FAQ">
                <input class="FAQ__input" type="checkbox" id="faq3" />
                <label class="FAQ__question" for="faq3">
                    I need Combat Eye Protection, and I can’t see how to order it.
                <div class="FAQ__svg-container">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 25 22">
                        <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z" />
                    </svg>
                </div>
                </label>
                <div class="FAQ__answer-box">
                    <p class="FAQ__answer">
                        Military Combat Eye Protection (MCEP) is a unit supply item. You can order inserts needed for MCEP through this app or a clinic, but the MCEP is provided by your unit. To order MCEP Inserts, click here.
                    </p>
                </div>
            </div>

            <div class="FAQ">
                <input class="FAQ__input" type="checkbox" id="faq4" />
                <label class="FAQ__question" for="faq4">
                    I need to enter a prescription number that isn’t an option. How do I enter it?
                <div class="FAQ__svg-container">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 25 22">
                        <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z" />
                    </svg>
                </div>
                </label>
                <div class="FAQ__answer-box">
                    <p class="FAQ__answer">
                        If you have a number you can’t enter because it’s out of the range of the options, you’ll have to go to your clinic to place the first order. The entered ranges cover most of our patients and minimize errors with entering data, but some patients will still need to go to a clinic for the first order.
                    </p>
                </div>
            </div>

            <div class="FAQ">
                <input class="FAQ__input" type="checkbox" id="faq5" />
                <label class="FAQ__question" for="faq5">
                    I’m a Soldier in the Reserves or National Guard, and it says that I can’t order. Why is that?
                <div class="FAQ__svg-container">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 25 22">
                        <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z" />
                    </svg>
                </div>
                </label>
                <div class="FAQ__answer-box">
                    <p class="FAQ__answer">
                        Your unit has to reimburse the DOD Optical Fabrication Enterprise for any optical devices that are ordered unless you are activated for 30 days or more. Therefore, your unit should place any necessary optical device orders.
                    </p>
                </div>
            </div>

            <div class="FAQ">
                <input class="FAQ__input" type="checkbox" id="faq6" />
                <label class="FAQ__question" for="faq6">
                    I’m a Soldier in the Reserves or National Guard, and it says that I can’t order, but I’ve been activated for > 30 days. How do I order?
                <div class="FAQ__svg-container">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 25 22">
                        <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z" />
                    </svg>
                </div>
                </label>
                <div class="FAQ__answer-box">
                    <p class="FAQ__answer">
                        If you were activated in the last 30 days, the system doesn’t know that you’ll be activated for >30 days yet. So you can order at your local Optometry Clinic at your convenience, or wait until you have been activated for more than 30 days. If you were activated more than 30 days ago, the system doesn’t recognize your activation date. Just take your orders and recent prescription and head to an Optometry Clinic, and they will be happy to help you order!
                    </p>
                </div>
            </div>

            <div class="FAQ">
                <input class="FAQ__input" type="checkbox" id="faq7" />
                <label class="FAQ__question" for="faq7">
                    I’m eligible to order glasses, but it says I can’t. What do I do?
                <div class="FAQ__svg-container">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 25 22">
                        <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z" />
                    </svg>
                </div>
                </label>
                <div class="FAQ__answer-box">
                    <p class="FAQ__answer">
                        JSPECS is only intended for Active Duty and Retirees, as well as Reservists and National Guard activated for more than 30 days. If you are in one of those categories and you can’t order, the system isn’t reflecting your status correctly. Follow up with the DEERS office to ensure your status is correct. If you’re in another category that is eligible to order, you must go through a clinic to order. Always, if you’re eligible to order, you can go to your Optometry Clinic to order!
                    </p>
                </div>
            </div>

            <div class="FAQ">
                <input class="FAQ__input" type="checkbox" id="faq8" />
                <label class="FAQ__question" for="faq8">
                    I have a question that isn't answered here. Who do I contact to get my question answered?
                <div class="FAQ__svg-container">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 25 22">
                        <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z" />
                    </svg>
                </div>
                </label>
                <div class="FAQ__answer-box">
                    <p class="FAQ__answer">
                        If you have a question that isn't answered in this FAQ, contact your local optometry clinic. The experts in your local optometry clinic will be able to assist you in answering your questions and/or ordering any devices you may need.
                    </p>
                </div>
            </div>

        </article>
</asp:Content>
